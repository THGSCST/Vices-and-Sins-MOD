using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Names2CSV
{
    internal class Names
    {
        string[][] ethnicNames;
        string[] nickNames;

        public Names(MemoryStream input)
        {
            using (BinaryReader br = new BinaryReader(input))
            {
                int totalEthnicGroups = br.ReadInt32();
                int totalNames = totalEthnicGroups * 3;
                ethnicNames = new string[totalNames][];

                for (int i = 0; i < totalNames; i++)
                {
                    int namesListTotal = br.ReadInt32();
                    ethnicNames[i] = new string[namesListTotal];
                    for (int j = 0; j < namesListTotal; j++)
                    {
                        ethnicNames[i][j] = ReadFixedLengthString(br);
                    }
                }

                int totalNicknames = br.ReadInt32();
                nickNames = new string[totalNicknames];
                for (int i = 0; i < totalNicknames; i++)
                {
                    nickNames[i] = ReadFixedLengthString(br);
                }
            }
        }

        enum Gender
        {
            Male, Female
        }

        enum Type
        {
            FirstName, LastName, Nickname
        }

        enum EthnicGroup 
        {
            AfricanAmerican, Polish, Chinese, Jewish, Italian, Irish, Latino, German,
            Russian, Scandinavian, American1, American2, American3, American4, Special
        }

        public string GetColumnHeader(int columnIndex)
        {
            if (columnIndex == 45) return "Nicknames";

            string[] genders = { "MaleFirstName", "FemaleFirstName", "LastName" };
            int totalEthnicGroups = Enum.GetValues(typeof(EthnicGroup)).Length;

            if (columnIndex < 0 || columnIndex >= totalEthnicGroups * 3)
                throw new ArgumentOutOfRangeException(nameof(columnIndex), "Invalid column index");

            string groupName = Enum.GetName(typeof(EthnicGroup), columnIndex / 3);
            return $"{groupName}{genders[columnIndex % 3]}";
        }

        public void SerializeToBinary(MemoryStream output)
        {
            using (BinaryWriter bw = new BinaryWriter(output))
            {
                // Calculate total number of ethnic groups
                int totalEthnicGroups = ethnicNames.Length / 3;
                bw.Write(totalEthnicGroups);

                // Write ethnic names
                foreach (var namesList in ethnicNames)
                {
                    bw.Write(namesList.Length);
                    foreach (var name in namesList)
                    {
                        WriteFixedLengthString(bw, name);
                    }
                }

                // Write nicknames
                bw.Write(nickNames.Length);
                foreach (var nickName in nickNames)
                {
                    WriteFixedLengthString(bw, nickName);
                }
            }
        }
        public Names(string csv)
        {
            var separator = new char[] { ',', ';' };
            var lines = csv.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            if (lines.Length < 2)
                throw new ArgumentException("CSV does not contain enough data.");

            var headers = lines[0].Split(separator, StringSplitOptions.None);

            // Assuming the last column is for nickNames
            int ethnicNamesColumns = headers.Length - 1;

            // Determine the number of rows for ethnicNames and nickNames
            int maxRows = lines.Length - 1;
            List<List<string>> ethnicNamesList = new List<List<string>>(ethnicNamesColumns);
            for (int i = 0; i < ethnicNamesColumns; i++)
            {
                ethnicNamesList.Add(new List<string>());
            }
            List<string> nickNamesList = new List<string>();

            for (int i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(separator, StringSplitOptions.None);
                for (int j = 0; j < values.Length; j++)
                {
                    if (j < ethnicNamesColumns)
                    {
                        if (!string.IsNullOrEmpty(values[j]))
                        {
                            ethnicNamesList[j].Add(values[j]);
                        }
                    }
                    else if (j == ethnicNamesColumns)
                    {
                        if (!string.IsNullOrEmpty(values[j]))
                        {
                            nickNamesList.Add(values[j]);
                        }
                    }
                }
            }

            // Convert lists to arrays
            ethnicNames = ethnicNamesList.Select(list => list.ToArray()).ToArray();
            nickNames = nickNamesList.ToArray();
        }


        public string ToCsv()
        {
            var separator = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            int maxColumns = ethnicNames.Length + 1;
            int maxRows = ethnicNames.Max(names => names.Length);
            maxRows = Math.Max(maxRows, nickNames.Length);

            StringBuilder sb = new StringBuilder();

            // Add column headers
            for (int i = 0; i < maxColumns; i++)
            {
                sb.Append(GetColumnHeader(i));
                if (i < maxColumns - 1)
                    sb.Append(separator);
            }
            sb.AppendLine();

            // Add rows
            for (int i = 0; i < maxRows; i++)
            {
                for (int j = 0; j < maxColumns; j++)
                {
                    if (j < ethnicNames.Length && i < ethnicNames[j].Length)
                        sb.Append(ethnicNames[j][i]);
                    else if (j == maxColumns - 1 && i < nickNames.Length)
                        sb.Append(nickNames[i]);
                    if (j < maxColumns - 1)
                        sb.Append(separator);
                }
                if (i < maxRows - 1)
                    sb.AppendLine();
            }
            return sb.ToString();
        }


        string GetFullName(EthnicGroup ethnic, Gender gender, int firstNameIndex, int lastNameIndex, int nicknameIndex)
        {
            int last = (int)ethnic * 3 + 2;
            int first = gender == Gender.Male ? last - 2 : last - 1;
            return $"{ethnicNames[first][firstNameIndex]} \"{nickNames[nicknameIndex]}\" {ethnicNames[last][lastNameIndex]}";
        }

        static Encoding Windows1252 = Encoding.GetEncoding(1252);
        public static string ReadFixedLengthString(BinaryReader reader, int length = 32)
        {
            byte[] bytes = reader.ReadBytes(length);
            int nullIndex = Array.IndexOf(bytes, byte.MinValue);
            if (nullIndex >= 0) length = nullIndex;
            return Windows1252.GetString(bytes, 0, length);
        }

        private void WriteFixedLengthString(BinaryWriter bw, string str, int length = 32)
        {
            byte[] chars = Windows1252.GetBytes(str);
            Array.Resize(ref chars, length);
            bw.Write(chars);
        }
    }
}
