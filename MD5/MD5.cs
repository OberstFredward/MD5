using System;
using System.Collections;
using System.ComponentModel;
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

        static int[] s = {7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,
                             5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,
                             4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,
                             6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21};

        static public string hash(string message) //Implementierung mit einem BitArray
        {
            BitArray[][] messagePreparated = Preparation(message);

            UInt32 A0 = a0;
            UInt32 B0 = b0;
            UInt32 C0 = c0;
            UInt32 D0 = d0;

            for (int i = 0; i < messagePreparated.Length; i++) //Für jeden 512Bit Block
            {
                for (int k = 0; k < 4; k++) //4 Runden
                {
                    for (int m = 0; m < 16; m++) //ganzer nachrichtenblock
                    {
                        //------Backups erstellen-------
                        UInt32 BackupA0 = A0;
                        UInt32 BackupB0 = B0;
                        UInt32 BackupC0 = C0;
                        UInt32 BackupD0 = D0;
                        //-----Verschiebungsoperationen------
                        A0 = BackupD0; //D wird so A
                        D0 = BackupC0; //C wird zu D
                        C0 = BackupB0; //B wird zu C
                                       //---MD5 Operation für A wird zu B----
                        switch (k)
                        {
                            case 0:
                                //f1
                                B0 = (uint)(rotate(((uint)((uint)(((uint)(BackupA0 + f1(BackupB0, BackupC0, BackupD0)))+get_part_message(messagePreparated, i, m)) + K[m])), s[m]) + BackupB0);
                                break;
                            case 1:
                                //f2
                                B0 = (uint)(rotate(((uint)((uint)(((uint)(BackupA0 + f2(BackupB0, BackupC0, BackupD0))) + get_part_message(messagePreparated, i, m)) + K[m+16])), s[m+16]) + BackupB0);
                                break;
                            case 2:
                                //f3
                                B0 = (uint)(rotate(((uint)((uint)(((uint)(BackupA0 + f3(BackupB0, BackupC0, BackupD0))) + get_part_message(messagePreparated, i, m)) + K[m+32])), s[m+32]) + BackupB0);
                                break;
                            case 3:
                                //f4
                                B0 = (uint)(rotate(((uint)((uint)(((uint)(BackupA0 + f4(BackupB0, BackupC0, BackupD0))) + get_part_message(messagePreparated, i, m)) + K[m+48])), s[m+48]) + BackupB0);
                                break;
                        }
                    }
                }               
            }
            string hash = A0.ToString("X2") + B0.ToString("X2") + C0.ToString("X2") + D0.ToString("X2");
            return hash;
        }

        private static UInt32 rotate(UInt32 block, int value)
        {
            BitArray rotates = new BitArray(32);
            BitArray new_block = new BitArray(BitConverter.GetBytes(block));

            for (int i = 0; i < value; i++)
            {
                rotates[i] = new_block[i];
            }

            for (int i = value; i < 32; i++)
            {
                new_block[i - value] = new_block[value];
            }

            for (int i = 32 - value, j = 0; i < 32; i++, j++)
            {
                new_block[i] = rotates[j];
            }

            byte[] return_value = new byte[new_block.Length / 8];
            new_block.CopyTo(return_value, 0);
            return BitConverter.ToUInt32(return_value,0);
        }

        static private UInt32 get_part_message(BitArray[][] message, int index_1, int index_2)
        {
            byte[] new_message = new byte[message[index_1][index_2].Length / 8];
            
            message[index_1][index_2].CopyTo(new_message,0);
            return BitConverter.ToUInt32(new_message,0);
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

        private static BitArray[][] Preparation(string message) //Gibt die 512Blöcke zurück in schon bereits unterteilten 16*32Bit Blöcke
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
            UInt64 backupLengthInt = (ulong)backup.Length - 1; //Die Angehängte 1 wurde schon mitgezähle
            byte[] backupLengthByte = BitConverter.GetBytes(backupLengthInt);
            BitArray backupLength = new BitArray(backupLengthByte);
            if (backupLength.Length <= 64) //Die Länge der Nachricht darf in Bitform max 64Stellen haben
            {
                for (int i = 0; i < backupLength.Length; i++)
                {
                    completeBits[i] = backupLength[i];
                }
            }
            else return null;
            //--------Alles in 32Bit Blöcke teilen-------
            if (completeBits.Length % 512 == 0)
            {
                int NumberOf32s = completeBits.Length / 32;
                BitArray[] bloecke32 = new BitArray[NumberOf32s];
                for (int i = 0; i < NumberOf32s; i++)
                {
                    BitArray block32 = new BitArray(32);
                    for (int k = 0; k < 32; k++)
                    {
                        block32[k] = completeBits[k + i * 32];
                    }
                    bloecke32[i] = block32;
                }
                //-------Alles in einzelne 512 Blöcke teilen & die 32Bit Blöcke einfügen---------
                NumberOf512s = completeBits.Length / 512;
                BitArray[][] bloecke512 = new BitArray[NumberOf512s][];
                for (int i = 0; i < NumberOf512s; i++)
                {
                    BitArray[] block512 = new BitArray[16];
                    for (int k = 0; k < 16; k++)
                    {
                        block512[k] = bloecke32[k + i*16];
                    }
                    bloecke512[i] = block512;
                }
                return bloecke512;
            }
            else return null;
        }
    }
}
