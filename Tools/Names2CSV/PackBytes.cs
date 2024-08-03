using System.Diagnostics;
using System.IO;

namespace Names2CSV
{
    public static class PackBytes
    {
        public static byte[] Decode(byte[] input)
        {
            using (var output = new MemoryStream())
            {
                int i = 0;
                while (i < input.Length)
                {
                    byte headerByte = input[i++];

                    Debug.Assert(headerByte != 0); //No-Operation

                    if (headerByte < 128) //Literal run
                    {
                        output.Write(input, i, headerByte);
                        i += headerByte;
                    }
                    else //Repeated run
                    {
                        byte repeatByte = input[i++];
                        for (int j = headerByte; j < 256; j++)
                            output.WriteByte(repeatByte);
                    }
                }
                return output.ToArray();
            }
        }
        public static byte[] Encode(byte[] input)
        {
            using (var output = new MemoryStream())
            {
                int i = 0;
                while (i < input.Length)
                {
                    int runLength = 1;

                    // Check for repeated runs
                    while (i + runLength < input.Length && input[i] == input[i + runLength] && runLength < 128)
                    {
                        runLength++;
                    }

                    if (runLength >= 3) // Only use repeated run if 3 or more bytes are the same
                    {
                        output.WriteByte((byte)(256 - runLength)); // Header byte for repeated run
                        output.WriteByte(input[i]); // Repeated byte
                        i += runLength;
                    }
                    else // Literal run
                    {
                        int literalStart = i;
                        int literalLength = 0;

                        // Extend literal run until we find a repeated run of at least 3 bytes or we reach the max length
                        while (i < input.Length && (literalLength < 128 - 1) &&
                               (i + 2 >= input.Length || input[i] != input[i + 1] || input[i] != input[i + 2]))
                        {
                            i++;
                            literalLength++;
                        }

                        output.WriteByte((byte)literalLength); // Header byte for literal run
                        output.Write(input, literalStart, literalLength); // Write literal bytes
                    }
                }
                return output.ToArray();
            }
        }
    }
}
