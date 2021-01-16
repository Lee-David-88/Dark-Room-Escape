using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour {

    public Camera puzzleCamera;
    public Transform spawnLocation;

    public Texture2D[] images;
    public int blockPerLine = 4;
    public int shuffleLength = 20;
    public float defaultMoveDuration = 0.2f;
    public float shuffleMoveDuration = 0.1f;
    enum PuzzleState { Solved, Shuffling, InPlay };
    PuzzleState state;

    Block emptyBlock;
    Block[,] blocks;
    Queue<Block> inputs;
    bool blockIsMoving;
    int shuffleMovesRemaining;
    Vector2Int previousShuffleOffset;

    private void Awake() {
        CreatePuzzle();
    }
    private void Start() {
        StartShuffle();
    }

    public void Update() {
        if (state == PuzzleState.Solved && Input.GetKeyDown(KeyCode.Space)) {
            StartShuffle();
        }
        if (puzzleCamera != null) {
            puzzleCamera.orthographicSize = blockPerLine * 0.55f;
        }
    }

    void CreatePuzzle() {
        int randomInt = Random.Range(0, images.Length);
        blocks = new Block[blockPerLine, blockPerLine];
        Texture2D[,] imageSlices = ImageSlicer.GetSlices(images[randomInt], blockPerLine);
        for (int y = 0; y < blockPerLine; y++) {
            for (int x = 0; x < blockPerLine; x++) {
                GameObject blockObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
                blockObject.layer = LayerMask.NameToLayer("Puzzle Cam");
                blockObject.transform.position = spawnLocation.position * (blockPerLine - 1) * 0.5f + new Vector3(x, y, 0);
                blockObject.transform.parent = transform;

                Block block = blockObject.AddComponent<Block>();
                block.OnBlockPressed += PlayerMoveBlockInput;
                block.OnFinishMoving += OnBlockFinishMoving;
                block.Init(new Vector2Int(x, y), imageSlices[x,y]);
                blocks[x, y] = block;

                if (y == 0 && x == blockPerLine - 1) {
                    emptyBlock = block;
                }
            }
        }
        inputs = new Queue<Block>();
    }

    void PlayerMoveBlockInput(Block blockToMove) {
        if (state == PuzzleState.InPlay) {
            inputs.Enqueue(blockToMove);
            MakeNextPlayerMove();
        }
    }

    void MakeNextPlayerMove() {
        while (inputs.Count > 0 && !blockIsMoving) {
            MoveBlock(inputs.Dequeue(), defaultMoveDuration);
        }
    }

    void MoveBlock(Block blockToMove, float duration) {
        if ((blockToMove.coord - emptyBlock.coord).sqrMagnitude == 1) {

            blocks[blockToMove.coord.x, blockToMove.coord.y] = emptyBlock;
            blocks[emptyBlock.coord.x, emptyBlock.coord.y] = blockToMove;

            Vector2Int targetCoord = emptyBlock.coord;
            emptyBlock.coord = blockToMove.coord;
            blockToMove.coord = targetCoord;

            Vector2 targetPosition = emptyBlock.transform.position;
            emptyBlock.transform.position = blockToMove.transform.position;
            blockToMove.MoveToPosition(targetPosition, duration);
            blockIsMoving = true;
        }
    }

    void OnBlockFinishMoving() {
        blockIsMoving = false;
        CheckIfSolved();

        if (state == PuzzleState.InPlay) {
            MakeNextPlayerMove();
        } else if (state == PuzzleState.Shuffling) {
            if (shuffleMovesRemaining > 0) {
                MakeNextShuffleMove();
            } else {
                state = PuzzleState.InPlay;
            }
        }
    }

    void StartShuffle() {
        state = PuzzleState.Shuffling;
        shuffleMovesRemaining = shuffleLength;
        emptyBlock.gameObject.SetActive(false);
        MakeNextShuffleMove();
    }

    void MakeNextShuffleMove() {
        Vector2Int[] offsets = { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };
        int randomIndex = Random.Range(0, offsets.Length);

        for (int i = 0; i < offsets.Length; i++) {
            Vector2Int offset = offsets[(randomIndex + i) % offsets.Length];
            if (offset != previousShuffleOffset * -1) {
                Vector2Int moveBlockCoord = emptyBlock.coord + offset;

                if (moveBlockCoord.x >= 0 && moveBlockCoord.x < blockPerLine && moveBlockCoord.y >= 0 && moveBlockCoord.y < blockPerLine) {
                    MoveBlock(blocks[moveBlockCoord.x, moveBlockCoord.y], shuffleMoveDuration);
                    shuffleMovesRemaining--;
                    previousShuffleOffset = offset;
                    break;
                }
            } 
        }
    }
    void CheckIfSolved() {
        foreach(Block block in blocks) {
            if (!block.IsAtStartingCoord()) {
                return;
            }
        }

        state = PuzzleState.Solved;
        emptyBlock.gameObject.SetActive(true);
    }
}
