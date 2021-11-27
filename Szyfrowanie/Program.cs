using System;
using System.Collections.Generic;
using System.Text;

namespace Szyfrowanie
{
    class Program
    {
        const string key = "@#$%*(#%353%f5h4jl@)^4r62hn)(@*J$Rh@gJ$R)@J$!7aR02j4RJ)(84irj0@$j*u209$ju)(@RJ409";

        const byte min_ASCII = 32; // Spacja
        const byte max_ASCII = 126; // ~

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Klucz:");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(key);
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Podaj tekst do zaszyfrowania:");
            Console.ForegroundColor = ConsoleColor.Blue;
            string msg = Console.ReadLine();
            string result;
            if (Encrypt(msg, key, out result))
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Zaszyfrowano:");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(result);
            }
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Wcisnij dowolny przycisk aby odszyfrowac");
            Console.ReadKey();
            string result2;
            if (Decrypt(result, key, out result2))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Odszyfrowano:");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(result2);
            }
            while(true)
            {
                Console.ReadKey();
            }
        }

        static bool Encrypt(string _msg, string _key, out string _result)
        {
            _result = default;

            for (int i=0; i<_msg.Length; i++){
                byte ascii = (byte)_msg[i];

                if (ascii < min_ASCII || ascii > max_ASCII)
                    return false;

                byte localIndex = (byte)(ascii - min_ASCII);

                int k = i % key.Length;
                byte keyAscii = (byte)key[k];
                byte localKeyIndex = (byte)(keyAscii - min_ASCII);

                byte size = max_ASCII - min_ASCII;

                int sum = localIndex + localKeyIndex;
                int overflow = sum - size;

                int A = overflow;
                if (A % 2 == 0)
                {
                    A--;
                    if (A < 0)
                        A = 1;
                }
                int A2 = size - sum;
                if (A2 % 2 != 0)
                    A2--;
                List<byte> generatedChars = new List<byte>();
                if (overflow > 0){
                    generatedChars.Add((byte)(A + min_ASCII));
                    generatedChars.Add((byte)(size - overflow + min_ASCII));
                }else{
                    generatedChars.Add((byte)(A2 + min_ASCII));
                    generatedChars.Add((byte)(sum + min_ASCII));
                }
                _result += ASCIIEncoding.ASCII.GetString(generatedChars.ToArray());
            }
            return true;
        }

        static bool Decrypt(string _msg, string _key, out string _result)
        {
            _result = default;
            byte[,] allChars = new byte[(int)Math.Floor((double)_msg.Length / 2), 2];
            int k = 0;
            for(int i=0; i < _msg.Length; i++)
            {
                if (k == 0)
                    allChars[(int)Math.Floor((double)i / 2), 0] = (byte)_msg[i];
                else if (k == 1)
                    allChars[(int)Math.Floor((double)i / 2), 1] = (byte)_msg[i];
                k++;
                if (k == 2)
                    k = 0;
            }
            for (int i = 0; i < (int)Math.Floor((double)_msg.Length / 2); i++)
            {

                byte keyValue = (byte)key[i % key.Length];
                byte size = max_ASCII - min_ASCII;
                byte A = allChars[i, 0];
                byte B = allChars[i, 1];
                byte localA = (byte)(A - min_ASCII);
                byte localB = (byte)(B - min_ASCII);
                byte localKeyIndex = (byte)(keyValue - min_ASCII);
                byte localIndex;
                if (localA % 2 != 0)
                {
                    byte overflow = (byte)Math.Abs(localB - size);
                    int sum = size + overflow;
                    localIndex = (byte)(sum - localKeyIndex);
                }
                else
                {
                    localIndex = (byte)(localB - localKeyIndex);
                }
                _result += (char)(localIndex + min_ASCII);
            }
            return true;
        }
    }
}
