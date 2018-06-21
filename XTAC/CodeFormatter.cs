using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTAC
{
    class CodeFormatter
    {

        const int numSpaces = 5;
         

        //formats the code in string 's'
        public static string Format(string s)
        {

            int indentLevel = 0;
            int index = 0;
            int charIx = 0;
            char lastChar = ' ';

            StringBuilder code = new StringBuilder(s);
            while (index != code.Length)
            {
                
                if (code[index] == '{')
                {
                    //need to delete white space behind {
                    DeleteTrailing(code, ref index);
                    code.Insert(index, '\n');
                    code.Insert(index, '\r');
                    index += 2; 
                    InsertWhiteSpace(code, ref index, indentLevel);
                    indentLevel++;
                    code.Insert(index + 1, '\n');
                    code.Insert(index+1, '\r');
                    index += 3;
                    InsertWhiteSpace(code, ref index, indentLevel);
                }
                else if (code[index] == '}')
                {  
                    //insert a cr before the }
                    DeleteTrailing(code, ref index);                   
                    code.Insert(index, '\n');
                    code.Insert(index, '\r');
                    index += 2; //skip cr  
                    indentLevel--;
                    InsertWhiteSpace(code, ref index, indentLevel);
                    index++; 
                }
                else if (code[index] == ';')
                {
                    index++; //skip ;
                    code.Insert(index, '\n');
                    code.Insert(index, '\r');
                    index += 2;
                    InsertWhiteSpace(code, ref index, indentLevel); //insert padding for next line
                }
                else
                {
                    index++;
                }
            }
            return code.ToString();
        }
         

        /*
         *Trims off leading white space
         *Adds the appropriate amount back on
         *starting at index
         */
        public static void InsertWhiteSpace(StringBuilder codeStr, ref int index, int indentLevel)
        {

            if (index == codeStr.Length) { return; }

            //strip existing space off
            while (IsWhiteSpace(codeStr[index])) 
            {
                codeStr.Remove(index,1);
                if (index == codeStr.Length) { return; }
            }

            //add the correct amount back
            if (indentLevel > 0)
            {
                for (int i = 0; i < indentLevel * numSpaces; i++) { codeStr.Insert(index, ' '); index++; }
            }
        }

        //assumes index is pointing to a '{'
        //at the end, the index should still point to a '{'
        private static void DeleteTrailing(StringBuilder code, ref int index)
        {
            while (IsWhiteSpace(code[index - 1]))
            {
                code.Remove(index - 1, 1);
                index--;
            }
            
        }

        static bool IsWhiteSpace(char ch)
        {
            if (ch == ' ') return true;
            if (ch == '\r') return true;
            if (ch == '\n') return true;
            if (ch == '\t') return true;
//            if (ch == 'X') return true;

            return false;
        }
    }
}
