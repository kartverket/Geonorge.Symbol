﻿using System.Collections.Generic;
using System.Linq;
using ImageTracerNet.Extensions;

namespace ImageTracerNet.Vectorization
{
    //https://en.wikipedia.org/wiki/Indexed_color
    // Container for the color-indexed image before and tracedata after vectorizing
    internal static class ColorGrouping
    {
        // array[x][y] of palette colors
        //public IEnumerable<ColorGroup> ColorGroups { get; }
        //public int ImageWidth { get; }
        //public int ImageHeight { get; }

        // array[palettelength][4] RGBA color palette
        //public IReadOnlyList<ColorReference> Palette { get; }
        // tracedata
        //public IReadOnlyList<Layer<SegmentPath>> Layers { get; set; }

        //public PaddedPaletteImage(IEnumerable<ColorReference> colors, int height, int width, IReadOnlyList<ColorReference> palette)
        //{
        //    Palette = palette;
        //    ImageWidth = width;
        //    ImageHeight = height;

        //    ColorGroups = ConvertToPaddedPaletteColorGroups(colors);
        //}

        
        //private int PaddedWidth => ImageWidth + 2;
        //private int PaddedHeight => ImageHeight + 2;

        // Creating indexed color array arr which has a boundary filled with -1 in every direction
        // Imagine the -1's being ColorReference.Empty and the 0's being null.
        // Example: 4x4 image becomes a 6x6 matrix:
        // -1 -1 -1 -1 -1 -1
        // -1  0  0  0  0 -1
        // -1  0  0  0  0 -1
        // -1  0  0  0  0 -1
        // -1  0  0  0  0 -1
        // -1 -1 -1 -1 -1 -1
        private static IEnumerable<ColorReference[]> CreatePaddedColorMatrix(int width, int height)
        {
            return new ColorReference[height][].Initialize(i =>
            i == 0 || i == height - 1
                ? new ColorReference[width].Initialize(j => ColorReference.Empty)
                : new ColorReference[width].Initialize(j => ColorReference.Empty, 0, width - 1));
        }

        public static IEnumerable<ColorGroup> Convert(IEnumerable<ColorReference> colors, int width, int height, IReadOnlyList<ColorReference> palette)
        {
            // Indexed color array requires +2 to the original width and height
            var paddedWidth = width + 2;
            var paddedHeight = height + 2;
            var imageColorQueue = new Queue<ColorReference>(colors.AsParallel().AsOrdered().Select(c => c.FindClosest(palette)));
            var colorMatrix = CreatePaddedColorMatrix(paddedWidth, paddedHeight).SelectMany(c => c).Select(c => c ?? imageColorQueue.Dequeue()).ToList();
            for (var row = 1; row < paddedHeight - 1; row++)
            {
                for (var column = 1; column < paddedWidth - 1; column++)
                {
                    yield return new ColorGroup(colorMatrix, row, column, paddedWidth);
                }
            }
        }

        // THIS IS NOW UNUSED
        // 1. Color quantization repeated "cycles" times, based on K-means clustering
        // https://en.wikipedia.org/wiki/Color_quantization
        // https://en.wikipedia.org/wiki/K-means_clustering
        //private static PaddedPaletteImage ColorQuantization(ImageData imageData, Color[] colorPalette, Options options)
        //{
        //    var arr = CreateIndexedColorArray(imageData.Height, imageData.Width);
        //    // Repeat clustering step "cycles" times
        //    for (var cycleCount = 0; cycleCount < options.ColorQuantization.ColorQuantCycles; cycleCount++)
        //    {
        //        // Reseting palette accumulator for averaging
        //        var accumulatorPaletteIndexer = Enumerable.Range(0, colorPalette.Length).ToDictionary(i => i, i => new PaletteAccumulator());

        //        for (var j = 0; j < imageData.Height; j++)
        //        {
        //            for (var i = 0; i < imageData.Width; i++)
        //            {
        //                var pixel = imageData.Colors[j * imageData.Width + i];
        //                var distance = 256 * 4;
        //                var paletteIndex = 0;
        //                // find closest color from palette by measuring (rectilinear) color distance between this pixel and all palette colors
        //                for (var k = 0; k < colorPalette.Length; k++)
        //                {
        //                    var color = colorPalette[k];
        //                    // In my experience, https://en.wikipedia.org/wiki/Rectilinear_distance works better than https://en.wikipedia.org/wiki/Euclidean_distance
        //                    var newDistance = color.CalculateRectilinearDistance(pixel);

        //                    if (newDistance >= distance) continue;

        //                    distance = newDistance;
        //                    paletteIndex = k;
        //                }

        //                // add to palettacc
        //                accumulatorPaletteIndexer[paletteIndex].Accumulate(pixel);
        //                arr[j + 1][i + 1] = paletteIndex;
        //            }
        //        }

        //        // averaging paletteacc for palette
        //        for (var k = 0; k < colorPalette.Length; k++)
        //        {
        //            // averaging
        //            if (accumulatorPaletteIndexer[k].A > 0) // Non-transparent accumulation
        //            {
        //                colorPalette[k] = accumulatorPaletteIndexer[k].CalculateAverage();
        //            }

        //            //https://github.com/jankovicsandras/imagetracerjava/issues/2
        //            // Randomizing a color, if there are too few pixels and there will be a new cycle
        //            if (cycleCount >= options.ColorQuantization.ColorQuantCycles - 1) continue;
        //            var ratio = accumulatorPaletteIndexer[k].Count / (double)(imageData.Width * imageData.Height);
        //            if ((ratio < options.ColorQuantization.MinColorRatio) && (cycleCount < options.ColorQuantization.ColorQuantCycles - 1))
        //            {
        //                colorPalette[k] = ColorExtensions.RandomColor();
        //            }
        //        }
        //    }

        //    return new PaddedPaletteImage(arr, colorPalette);
        //}
    }
}
