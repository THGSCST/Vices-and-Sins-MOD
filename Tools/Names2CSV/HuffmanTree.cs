using System;
using System.Collections.Generic;
using System.IO;

namespace Names2CSV
{
    public static class HuffmanTree
    {
        private class HuffmanNode : IComparable<HuffmanNode>
        {
            public HuffmanNode Left { get; set; }
            public HuffmanNode Right { get; set; }
            public byte Symbol { get; set; }
            public int Frequency { get; set; }

            public bool IsLeaf => Left == null && Right == null;

            public HuffmanNode(byte symbol = 0, int frequency = 0)
            {
                Symbol = symbol;
                Frequency = frequency;
            }

            public HuffmanNode(HuffmanNode left, HuffmanNode right)
            {
                Left = left;
                Right = right;
                Frequency = left.Frequency + right.Frequency;
            }

            public HuffmanNode(MemoryStream data)
            {
                byte children = (byte)data.ReadByte();
                for (int i = 0; i < children; i++)
                {
                    byte direction = (byte)data.ReadByte();
                    if (direction == (byte)'L')
                        Left = new HuffmanNode(data);
                    else if (direction == (byte)'R')
                        Right = new HuffmanNode(data);
                }
                Symbol = (byte)data.ReadByte();
            }

            public int CompareTo(HuffmanNode other)
            {
                if (other == null) return 1;
                return Frequency.CompareTo(other.Frequency) != 0
                    ? Frequency.CompareTo(other.Frequency)
                    : Symbol.CompareTo(other.Symbol);
            }
        }

        public static void Decode(MemoryStream input, MemoryStream output)
        {
            var root = new HuffmanNode(input);
            var currentNode = root;
            int currentByte;

            while ((currentByte = input.ReadByte()) != -1)
            {
                for (int bitMask = 128; bitMask != 0; bitMask >>= 1)
                {
                    currentNode = (currentByte & bitMask) == 0 ? currentNode.Left : currentNode.Right;

                    if (currentNode.IsLeaf)
                    {
                        output.WriteByte(currentNode.Symbol);
                        currentNode = root;
                    }
                }
            }
        }

        private static SortedDictionary<byte, int> CalculateFrequencies(MemoryStream memoryStream)
        {
            memoryStream.Position = 0;
            var frequencies = new SortedDictionary<byte, int>();
            int byteValue;

            while ((byteValue = memoryStream.ReadByte()) != -1)
            {
                if (!frequencies.ContainsKey((byte)byteValue))
                    frequencies[(byte)byteValue] = 0;

                frequencies[(byte)byteValue]++;
            }

            return frequencies;
        }

        private static HuffmanNode BuildTree(SortedDictionary<byte, int> frequencies)
        {
            var nodeList = new List<HuffmanNode>();
            foreach (var kvp in frequencies)
                nodeList.Add(new HuffmanNode(kvp.Key, kvp.Value));

            while (nodeList.Count > 1)
            {
                nodeList.Sort();
                var left = nodeList[0];
                var right = nodeList[1];

                nodeList.RemoveRange(0, 2);

                var parent = new HuffmanNode(left, right);
                nodeList.Add(parent);
            }

            return nodeList[0];
        }

        private static void GenerateCodes(HuffmanNode node, List<bool> code, Dictionary<byte, List<bool>> codes)
        {
            if (node.IsLeaf)
            {
                codes[node.Symbol] = new List<bool>(code);
                return;
            }

            code.Add(false);
            GenerateCodes(node.Left, code, codes);
            code.RemoveAt(code.Count - 1);

            code.Add(true);
            GenerateCodes(node.Right, code, codes);
            code.RemoveAt(code.Count - 1);
        }

        private static Dictionary<byte, List<bool>> GenerateHuffmanCodes(HuffmanNode root)
        {
            var huffmanCodes = new Dictionary<byte, List<bool>>();
            GenerateCodes(root, new List<bool>(), huffmanCodes);
            return huffmanCodes;
        }

        public static void Encode(MemoryStream input, MemoryStream output)
        {
            var frequencies = CalculateFrequencies(input);
            var root = BuildTree(frequencies);
            SerializeNode(root, output);
            var huffmanCodes = GenerateHuffmanCodes(root);

            input.Position = 0;

            byte current = 0;
            byte bitValue = 128;
            int byteValue;

            while ((byteValue = input.ReadByte()) != -1)
            {
                var code = huffmanCodes[(byte)byteValue];
                foreach (var bit in code)
                {
                    if (bit) current += bitValue;
                    bitValue >>= 1;
                    if (bitValue == 0)
                    {
                        output.WriteByte(current);
                        current = 0;
                        bitValue = 128;
                    }
                }
            }

            if (bitValue != 128)
            {
                var zero = huffmanCodes[0];
                foreach (var bit in zero)
                {
                    if (bit) current += bitValue;
                    bitValue >>= 1;
                }
                output.WriteByte(current);
            }
        }

        private static void SerializeNode(HuffmanNode node, MemoryStream stream)
        {
            if (node.Left == null && node.Right == null)
            {
                stream.WriteByte(0);
            }
            else
            {
                stream.WriteByte(2);
                if (node.Left != null)
                {
                    stream.WriteByte((byte)'L');
                    SerializeNode(node.Left, stream);
                }
                if (node.Right != null)
                {
                    stream.WriteByte((byte)'R');
                    SerializeNode(node.Right, stream);
                }
            }
            stream.WriteByte(node.Symbol);
        }
    }
}
