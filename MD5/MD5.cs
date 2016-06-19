using System;
using System.Collections;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace MD5
{
    static class MD5
    {
        private static UTF8Encoding encoding = new UTF8Encoding();

        const UInt32 a0 = 0x67452301;
        const UInt32 b0 = 0xEFCDAB89;
        const UInt32 c0 = 0x98BADCFE;
        const UInt32 d0 = 0x10325476;

        static UInt32[] M = new UInt32[16];

        static UInt32[] K = {0xd76aa478, 0xe8c7b756, 0x242070db, 0xc1bdceee,
                             0xf57c0faf, 0x4787c62a, 0xa8304613, 0xfd469501,
                             0x698098d8, 0x8b44f7af, 0xffff5bb1, 0x895cd7be,
                             0x6b901122, 0xfd987193, 0xa679438e, 0x49b40821,
                             0xf61e2562, 0xc040b340, 0x265e5a51, 0xe9b6c7aa,
                             0xd62f105d, 0x02441453, 0xd8a1e681, 0xe7d3fbc8,
                             0x21e1cde6, 0xc33707d6, 0xf4d50d87, 0x455a14ed,
                             0xa9e3e905, 0xfcefa3f8, 0x676f02d9, 0x8d2a4c8a,
                             0xfffa3942, 0x8771f681, 0x6d9d6122, 0xfde5380c,
                             0xa4beea44, 0x4bdecfa9, 0xf6bb4b60, 0xbebfbc70,
                             0x289b7ec6, 0xeaa127fa, 0xd4ef3085, 0x04881d05,
                             0xd9d4d039, 0xe6db99e5, 0x1fa27cf8, 0xc4ac5665,
                             0xf4292244, 0x432aff97, 0xab9423a7, 0xfc93a039,
                             0x655b59c3, 0x8f0ccc92, 0xffeff47d, 0x85845dd1,
                             0x6fa87e4f, 0xfe2ce6e0, 0xa3014314, 0x4e0811a1,
                             0xf7537e82, 0xbd3af235, 0x2ad7d2bb, 0xeb86d391};

        static UInt32[] s = {7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,
                             5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,
                             4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,
                             6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21};

        static public string hash(string message) //Implementierung mit einem BitArray
        {
            ArrayList bloecke512 = Preparation(message);
            for (int i = 0; i < bloecke512.Count; i++)
            {
     
            }
            return "";
        }

        //Die 'F' Methoden
        static private UInt32 f1(UInt32 X, UInt32 Y, UInt32 Z)
        {
            return (X & Y) | (~X & Z);
        }

        static private UInt32 f2(UInt32 X, UInt32 Y, UInt32 Z)
        {
            return (X & Z) | (X & ~Z);
        }

        static private UInt32 f3(UInt32 X, UInt32 Y, UInt32 Z)
        {
            return X ^ Y ^ Z;
        }

        static private UInt32 f4(UInt32 X, UInt32 Y, UInt32 Z)
        {
            return Y ^ (X | ~Z);
        }

        private static ArrayList Preparation(string message)
        {
            //--------Message Reverse-------------------------
            char[] CompleteMessageCharArray = message.ToCharArray();
            Array.Reverse(CompleteMessageCharArray); //Umdrehen der Chars
            message = new string(CompleteMessageCharArray);
            //--------Erstelle BitArray--------------
            BitArray completeBits = new BitArray(encoding.GetBytes(message));
            //-------SCHRITT1: logisch '1' anhängen-----------
            BitArray backup = completeBits;
            completeBits = new BitArray(backup.Length + 1);
            for (int i = 0; i < backup.Length; i++)
            {
                completeBits[i + 1] = backup[i];
            }
            completeBits[0] = true;
            //---------Mit logisch '0' auffüllen, bis es durch 512 teilbar ist (Auch wenn es durch 512 teilbar ist wird es noch einmal aufgefüllt [wegen dem nächsten Schritt])--------
            backup = completeBits;
            int NumberOf512s = completeBits.Length / 512;
            int NumberOfNulls = (NumberOf512s + 1) * 512 - completeBits.Length;
            completeBits = new BitArray(backup.Length + NumberOfNulls);
            for (int i = 0; i < completeBits.Length; i++)
            {
                if (i < NumberOfNulls)
                {
                    completeBits[i] = false;
                }
                else
                {
                    completeBits[i] = backup[i - NumberOfNulls];
                }
            }

            //---------Die Länge der ursprünglichen Nachricht anhängen (die hinteren '0' werden überschrieben)---------------
            UInt64 backupLengthInt = (ulong)backup.Length;
            byte[] backupLengthByte = BitConverter.GetBytes(backupLengthInt);
            int counter = 0;
            for (int i = backupLengthByte.Length - 1; i >= 0; i--) //'0' die wegen dem BitConverter entstehen löschen
            {
                if (backupLengthByte[i] == 0) counter++;
            }
            byte[] backupLengthBytesBackup = backupLengthByte;
            backupLengthByte = new byte[backupLengthBytesBackup.Length - counter];
            for (int i = 0; i < backupLengthByte.Length; i++)
            {
                backupLengthByte[i] = backupLengthBytesBackup[i];
            }
            BitArray backupLength = new BitArray(backupLengthByte);
            if (backupLength.Length <= 64) //Die Länge der Nachricht darf in Bitform max 64Stellen haben
            {
                for (int i = 0; i < backupLength.Length; i++)
                {
                    completeBits[i] = backupLength[i];
                }
            }
            else return null;
            //-------In einzelne 512 Blöcke teilen---------
            ArrayList bloecke512 = new ArrayList();
            BitArray[] blo
            if (completeBits.Length%512 == 0)
            {
                NumberOf512s = completeBits.Length/512;
                for (int i = 0; i < NumberOf512s; i++)
                {
                    BitArray block512 = new BitArray(512);
                    for (int k = 0; k < 512; k++)
                    {
                        block512[k] = completeBits[k+(i*512)];
                    }
                    bloecke512.Add(block512);
                }
            }
            else return null;
            //--------Jeden 512 Block in 16 32Blöcke teilen-------
            for (int i = 0; i < bloecke512.Count; i++) //Anzahl der 512blöcke
            {
                //TODO: HIER WEITERMACHEN
            }
            return bloecke512;
        }
    }
}
