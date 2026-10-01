using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GeneticSearch
{
    class Program
    {
        struct Protein
        {
            public string name;
            public string organism;
            public string amino_acids;
        }

        struct Command
        {
            public string name;
            public string parameter1;
            public string parameter2;
        }

        static List<Command> ReadCommands(string filename)
        {
            StreamReader reader = new StreamReader(filename);
            List<Command> commands = new List<Command>();

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split('\t');
                Command command = new Command();
                command.name = parts[0];
                command.parameter1 = parts.Length > 1 ? parts[1] : string.Empty;
                command.parameter2 = parts.Length > 2 ? parts[2] : string.Empty;
                commands.Add(command);
            }
            reader.Close();
            return commands;
        }

        static List<Protein> ReadData(string filename)
        {
            StreamReader reader = new StreamReader(filename);
            List<Protein> data = new List<Protein>();

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split('\t');
                Protein protein = new Protein();
                protein.name = parts[0];
                protein.organism = parts[1];
                protein.amino_acids = parts[2];
                data.Add(protein);
            }
            reader.Close();
            return data;
        }

        static string EncodingRLE(string amino_acids)
        {
            StringBuilder encoded = new StringBuilder();
            for (int i = 0; i < amino_acids.Length; i++)
            {
                char ch = amino_acids[i];
                int count = 1;
                while (i < amino_acids.Length - 1 && amino_acids[i + 1] == ch)
                {
                    count++;
                    i++;
                }
                if (count > 2)
                    encoded.Append(count).Append(ch);
                else if (count == 2)
                    encoded.Append(ch).Append(ch);
                else
                    encoded.Append(ch);
            }
            return encoded.ToString();
        }

        static string DecodingRLE(string amino_acids)
        {
            StringBuilder decoded = new StringBuilder();
            for (int i = 0; i < amino_acids.Length; i++)
            {
                char ch = amino_acids[i];
                if (char.IsDigit(ch))
                {
                    char letter = amino_acids[i + 1];
                    int count = ch - '0';
                    for (int j = 1; j < count; j++)
                    {
                        decoded.Append(letter);
                    }
                }
                else
                    decoded.Append(ch);
            }
            return decoded.ToString();
        }

        static void CommandHandler(List<Protein> proteins, List<Command> commands, string outputFilename)
        {
            using (StreamWriter writer = new StreamWriter(outputFilename, false, Encoding.UTF8))
            {
                writer.WriteLine("Ivan Ivanov");
                writer.WriteLine("Генетический поиск");
                writer.WriteLine(new string('-', 74));

                for (int i = 0; i < commands.Count; i++)
                {
                    int cmdNumber = i + 1;
                    Command cmd = commands[i];

                    if (cmd.name == "search")
                    {
                        writer.WriteLine($"{cmdNumber:D3}   search   {cmd.parameter1}");
                        writer.WriteLine("organism                 protein");

                        string decodedSearch = DecodingRLE(cmd.parameter1);
                        bool found = false;

                        foreach (var p in proteins)
                        {
                            if (DecodingRLE(p.amino_acids).Contains(decodedSearch))
                            {
                                writer.WriteLine($"{p.organism,-24} {p.name}");
                                found = true;
                            }
                        }

                        if (!found)
                        {
                            writer.WriteLine("NOT FOUND");
                        }
                    }
                    else if (cmd.name == "diff")
                    {
                        writer.WriteLine($"{cmdNumber:D3}   diff   {cmd.parameter1}   {cmd.parameter2}");
                        writer.WriteLine("amino-acids difference:");

                        int p1Index = proteins.FindIndex(p => p.name == cmd.parameter1);
                        int p2Index = proteins.FindIndex(p => p.name == cmd.parameter2);

                        if (p1Index == -1 && p2Index == -1)
                        {
                            writer.WriteLine($"MISSING: {cmd.parameter1}, {cmd.parameter2}");
                        }
                        else if (p1Index == -1)
                        {
                            writer.WriteLine($"MISSING: {cmd.parameter1}");
                        }
                        else if (p2Index == -1)
                        {
                            writer.WriteLine($"MISSING: {cmd.parameter2}");
                        }
                        else
                        {
                            string seq1 = DecodingRLE(proteins[p1Index].amino_acids);
                            string seq2 = DecodingRLE(proteins[p2Index].amino_acids);

                            int diffCount = Math.Abs(seq1.Length - seq2.Length);
                            int minLen = Math.Min(seq1.Length, seq2.Length);

                            for (int j = 0; j < minLen; j++)
                            {
                                if (seq1[j] != seq2[j]) diffCount++;
                            }
                            writer.WriteLine(diffCount);
                        }
                    }
                    else if (cmd.name == "mode")
                    {
                        writer.WriteLine($"{cmdNumber:D3}   mode   {cmd.parameter1}");
                        writer.WriteLine("amino-acid occurs:");

                        int pIndex = proteins.FindIndex(p => p.name == cmd.parameter1);

                        if (pIndex == -1)
                        {
                            writer.WriteLine($"MISSING: {cmd.parameter1}");
                        }
                        else
                        {
                            string seq = DecodingRLE(proteins[pIndex].amino_acids);
                            var mostFrequent = seq.GroupBy(c => c)
                                                  .OrderByDescending(g => g.Count())
                                                  .ThenBy(g => g.Key)
                                                  .First();

                            writer.WriteLine($"{mostFrequent.Key}          {mostFrequent.Count()}");
                        }
                    }
                    writer.WriteLine(new string('-', 74));
                }
            }
        }

        static void Main(string[] args)
        {
            string sequencesFile = "sequences.2.txt";
            string commandsFile = "commands.2.txt";
            string outputFile = "genedata.txt";

            try
            {
                List<Protein> data = ReadData(sequencesFile);
                List<Command> commands = ReadCommands(commandsFile);

                CommandHandler(data, commands, outputFile);

                Console.WriteLine($"Обработка завершена. Результаты сохранены в файл: {outputFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обработке файлов: {ex.Message}");
            }
        }
    }
}

