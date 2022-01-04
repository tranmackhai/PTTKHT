using System;

namespace BTL.utils
{
    public class Method
    {
        public string addZero(int num, int value)
        {
            string result = "";
            for (int i = value.ToString().Length; i < num; i++)
            {
                result += "0";
            }
            return result + value;
        }
    }
}
