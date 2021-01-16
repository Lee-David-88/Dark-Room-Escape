using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageSlicer {
    
    public static Texture2D[,] GetSlices(Texture2D image, int blockPerLine) {
        int imageSize = Mathf.Min(image.width, image.height);
        int blockSize = imageSize / blockPerLine;

        Texture2D[,] blocks = new Texture2D[blockPerLine, blockPerLine];
        for (int y = 0; y < blockPerLine; y++) {
            for (int x = 0; x < blockPerLine; x++) {
                Texture2D block = new Texture2D(blockSize, blockSize);
                block.wrapMode = TextureWrapMode.Clamp;
                block.SetPixels(image.GetPixels(x * blockSize, y * blockSize, blockSize, blockSize));
                block.Apply();
                blocks[x, y] = block;
            }
        }
        return blocks;
    }
}
