using System;
using System.Text;



namespace DES
{
    class Fanction
    {
        public string EncryptionStart(string text, string key)
        {
            /// ключ на вход 64 бита, 8 символов
            
            string binary_key = FromTextToBinaryk(key);
            if (binary_key == "mis")
            {
                return ("неверный формат ключа");
            }
            else
            {
                ///получение 56-битного ключа, перестановка
                string key_plus = DoPermutation(binary_key, DESData.pc_1);

                string C0 = "", D0 = "";

                ///левая и правая половинки ключа по 28 бит
                C0 = SetLeftHalvesKey(key_plus);
                D0 = SetRightHalvesKey(key_plus);

                /// все ключи
                Keys keys = SetAllKeys(C0, D0);


                string binaryText = "";

               
                binaryText = FromTextToBinary(text);
                

                binaryText = setTextMutipleOf64Bits(binaryText);

                StringBuilder EncryptedTextBuilder = new StringBuilder(binaryText.Length);

                for (int i = 0; i < (binaryText.Length / 64); i++)
                {
                    //Исходный текст T (блок 64 бит) преобразуется c помощью начальной перестановки IP
                    string PermutatedText = DoPermutation(binaryText.Substring(i * 64, 64), DESData.ip);/// с блока i еще 64 символов следующий блок

                    string L0 = "", R0 = "";

                    //разбиваем исходный блок текста по 32 бита
                    L0 = SetLeftHalvesKey(PermutatedText);
                    R0 = SetRightHalvesKey(PermutatedText);

                    //начало шифровки
                    string FinalText = FinalEncription(L0, R0, keys, false);

                    //объединение зашифрованных блоков
                    EncryptedTextBuilder.Append(FinalText);
                }

                return EncryptedTextBuilder.ToString();
            }
        }


        public string DecryptionStart(string text, string key)
        {
            string binary_key = FromTextToBinaryk(key);
            if (binary_key == "mis")
            {
                return ("неверный формат ключа");
            }
            else
            {
                //получаем 56-битный ключ
                string key_plus = DoPermutation(binary_key, DESData.pc_1);

                string C0 = "", D0 = "";

                //левая и правая половинки ключа по 28 бит
                C0 = SetLeftHalvesKey(key_plus);
                D0 = SetRightHalvesKey(key_plus);
                
                //получаем все ключи 
                Keys keys = SetAllKeys(C0, D0);

                string binaryText = "";



                binaryText = text;


                binaryText = setTextMutipleOf64Bits(binaryText);

                StringBuilder DecryptedTextBuilder = new StringBuilder(binaryText.Length);

                for (int i = 0; i < (binaryText.Length / 64); i++)
                {
                    string PermutatedText = DoPermutation(binaryText.Substring(i * 64, 64), DESData.ip);

                    string L0 = "", R0 = "";

                    L0 = SetLeftHalvesKey(PermutatedText);
                    R0 = SetRightHalvesKey(PermutatedText);

                    string FinalText = FinalEncription(L0, R0, keys, true);

                    if ((i * 64 + 64) == binaryText.Length)/// i с нуля, поэтому компенсируем +64, смотрим на последний блок
                    {
                        StringBuilder last_text = new StringBuilder(FinalText.TrimEnd('0'));

                        int count = FinalText.Length - last_text.Length;

                        if ((count % 16) != 0)
                        {
                            count = 16 - (count % 16);
                        }

                        string append_text = "";

                        for (int k = 0; k < count; k++)
                        {
                            append_text += "0";
                        }

                        DecryptedTextBuilder.Append(last_text.ToString() + append_text);
                    }
                    else
                    {
                        DecryptedTextBuilder.Append(FinalText);
                    }

                }

                return FromBinaryToText(DecryptedTextBuilder.ToString());
            }
        }

        public string FromTextToBinary(string text)
        {
            string output = "";

            for (int i = 0; i < text.Length; i++)
            {
                string char_binary = Convert.ToString(text[i], 2);

                while (char_binary.Length < 16)
                    char_binary = "0" + char_binary;

                output += char_binary;
            }

            return output;
        }
        public string FromTextToBinaryk(string text)
        {
            string output = "";

            for (int i = 0; i < text.Length; i++)
            {
                string char_binary = Convert.ToString(text[i], 2);
                if (char_binary.Length > 8)
                {
                    output = "mis";
                    break;
                }
                else
                {
                    while (char_binary.Length < 8)
                        char_binary = "0" + char_binary;

                    output += char_binary;
                }
            }

            return output;
        }
        
       
        public static string FromDeciamlToBinary(int binary)
        {
           

            string binarystring = "";
            int factor = 128;

            for (int i = 0; i < 8; i++)
            {
                if (binary >= factor)
                {
                    binary -= factor;
                    binarystring += "1";
                }
                else
                {
                    binarystring += "0";
                }
                factor /= 2;
            }

            return binarystring;
        }

       
        public string setTextMutipleOf64Bits(string text)
        {
            if ((text.Length % 64) != 0)/// дополняем блоки нулями для ровных 64 битовых блоков
            {
                int maxLength = 0;
                maxLength = ((text.Length / 64) + 1) * 64;
                text = text.PadRight(maxLength, '0');
            }

            return text;
        }
        public string FromBinaryToText(string binarystring)
        {
            StringBuilder text = new StringBuilder(binarystring.Length / 16);

            for (int i = 0; i < (binarystring.Length / 16); i++)
            {
                string word = binarystring.Substring(i * 16, 16);
                
                
                    
               text.Append((char)Convert.ToInt32(word, 2));
                
              
            }

            return text.ToString();
        }
        public string DoPermutation(string text, int[] order)///
        {
            StringBuilder PermutatedText = new StringBuilder(order.Length);
            for (int i = 0; i < order.Length; i++)
            {
                //перестановка, так как в таблицах номера с 1, а в сообщении индекс-номера с 0, то нужно вычесть -1
                PermutatedText.Append(text[order[i] - 1]);
            }
            return PermutatedText.ToString();
        }

        //For SBoxes Transformation
        public string DoPermutations(string text, int[,] order)
        {
            string PermutatedText = "";
            //1-ый бит и 6-ой бит - (число)индекс строки в блоке Si
            int rowIndex = Convert.ToInt32(text[0].ToString() + text[text.Length - 1].ToString(), 2);
            //2,3,4,5 биты - (число)индекс столбца в блоке
            int colIndex = Convert.ToInt32(text.Substring(1, 4), 2);
            //получаем двоичное представление десятичного числа, полученного из таблицы Si по индексам [rowIndex,colIndex]
            PermutatedText = FromDeciamlToBinary(order[rowIndex, colIndex]);
            return PermutatedText;
        }
        public string SetLeftHalvesKey(string text)
        
        {
            int midindex = (text.Length / 2) - 1;
            string result = "";
            result = text.Substring(0, midindex + 1);
            return result;

        }

        public string SetRightHalvesKey(string text)
        {
            int midindex = (text.Length / 2) - 1;
            string result = "";
            result = text.Substring(midindex + 1);
            return result;
        }

        
        public Keys SetAllKeys(string C0, string D0)
        {
            Keys keys = new Keys();
            keys.Cn[0] = C0;
            keys.Dn[0] = D0;

            for (int i = 1; i < keys.Cn.Length; i++)
            {
                //один - два циклических сдвига
                keys.Cn[i] = LeftShift(keys.Cn[i - 1], DESData.nrOfShifts[i]);
                keys.Dn[i] = LeftShift(keys.Dn[i - 1], DESData.nrOfShifts[i]);
                //получаем ключи Ki (i=0...16), перестановкой бит
                keys.Kn[i - 1] = DoPermutation(keys.Cn[i] + keys.Dn[i], DESData.pc_2);
            }

            return keys;
        }
        
        public string LeftShift(string text, int count)
        {
            if (count < 1)
            {
                Console.WriteLine("The count of leftshift is must more than 1 time.");
                return null;
            }
            string temp = text.Substring(0, count);
            StringBuilder shifted = new StringBuilder(text.Length);
            //берем строку (из text) начиная с указанного индексса + сдвинутые влево биты
            shifted.Append(text.Substring(count) + temp);

            return shifted.ToString();
        }
        public string FinalEncription(string L0, string R0, Keys keys, bool IsReverse)
        {
            string Ln = "", Rn = "", Ln_1 = L0, Rn_1 = R0;
            
            if (IsReverse == true)
            {
                int i = 15;
                while (i>=0)
                {
                    Ln = Rn_1;
                    Rn = XOR(Ln_1, f(Rn_1, keys.Kn[i]));

                    //Следующий шаг после L1, R1 это L2 = R1, R2 = L1 + f(R1, K2),
                    //следовательно, значение шага1' Ln, Rn iэто Rn_1, Ln_1 в шаге 2..
                    Ln_1 = Ln;
                    Rn_1 = Rn;
                    i -= 1;
 
                }
            }
            else
            {
                int i = 0;
                while (i < 16)
                {
                    Ln = Rn_1;
                    Rn = XOR(Ln_1, f(Rn_1, keys.Kn[i]));

                    //Следующий шаг после L1, R1 это L2 = R1, R2 = L1 + f(R1, K2),
                    //следовательно, значение шага1' Ln, Rn iэто Rn_1, Ln_1 в шаге 2..
                    Ln_1 = Ln;
                    Rn_1 = Rn;
                    i += 1;

                }
            }
            
            

            string R16L16 = Rn + Ln;

            //Конечная перестановка IP−1 действует на T16 и используется для восстановления позиции.
            string Encripted_Text = DoPermutation(R16L16, DESData.ip_1);

            return Encripted_Text;
        }

        //если шифрование то ключи (i=0...15), иначе  (i=15...0)
        
        //Функция Фейстеля
        public string f(string Rn_1, string Kn)
        {
            //Функция Е расширяет 32-битовый вектор  до 48-битового
            
            string E_Rn_1 = E_Selection(Rn_1);

            //Битовое сложение по модулю 2 с ключом
            string XOR_Rn_1_Kn = XOR(E_Rn_1, Kn);

            // после перестановки блок складывается по модулю 2 с ключами ki
            
            string sBoxedText = sBox_Transform(XOR_Rn_1_Kn);
  
            string P_sBoxedText = P(sBoxedText);

            return P_sBoxedText;
        }
        
        public string P(string text)
        {
            string PermutatedText = "";

            PermutatedText = DoPermutation(text, DESData.pc_p);

            return PermutatedText;
        }
        
        public string sBox_Transform(string text)
        {
            StringBuilder TransformedText = new StringBuilder(32);

            for (int i = 0; i < 8; i++)
            {
                string temp = text.Substring(i * 6, 6);
                //В строку добавляется по 4 бита, полученные в результате трансформации Bj в B'j
                TransformedText.Append(DoPermutations(temp, DESData.sBoxes[i]));
            }

            return TransformedText.ToString();
        }
       
        public string E_Selection(string Rn_1)
        {
            string ExpandedText = DoPermutation(Rn_1, DESData.pc_e);

            return ExpandedText;
        }
        
        public string XOR(string text1, string text2)
        {
           
            StringBuilder XORed_Text = new StringBuilder(text1.Length);

            for (int i = 0; i < text1.Length; i++)
            {
                if (text1[i] != text2[i])   
                {
                    XORed_Text.Append("1");
                }
                else
                {
                    XORed_Text.Append("0");
                }
            }

            return XORed_Text.ToString();
        }
    }
}
