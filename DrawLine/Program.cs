using System;
using System.Collections.Generic;
using System.Linq;

namespace DrawLine
{
    internal class Program
    {
        static void Main(string[] args)
        { 
            string input = Console.ReadLine();
            string[] validInput = ValidateInput(input);
            decimal x, y;

            if (validInput.Contains("="))
            {
                if (validInput.Contains("x") && validInput.Contains("y"))
                {
                    decimal min = -1, max = 1;
                    decimal pace = 0.1M;
                    int Yindex = Array.IndexOf(validInput, "y");

                    for (decimal i = min; i < max; i += pace)
                    {
                        validInput[Yindex] = i.ToString();
                        y = i;

                        string[][] validEquation = ValidateEquation(validInput);
                        x = EquationSolver(validEquation);

                        Console.WriteLine($"({x}, {y})");
                    }
                }
            }
            else
            {
                string[] simpleInput = SimplifyInput(validInput);
                Console.WriteLine(Calculate(simpleInput));
            }
        }

        static string[] ValidateInput(string input)
        {
            List<string> output = new List<string>();

            bool canBeNumber = true;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '(' || input[i] == ')')
                {
                    output.Add(input[i].ToString());
                }
                else
                {
                    if (canBeNumber)
                    {
                        int j = i + 1;
                        string number = input[i].ToString();

                        while (j < input.Length && char.IsDigit(input[j]))
                        {
                            number += input[j].ToString();
                            j++;
                        }

                        canBeNumber = false;
                        output.Add(number);
                    }
                    else if (IsSignCorrect(input[i].ToString()) || char.IsLetter(input[i]))
                    {
                        canBeNumber = true;
                        output.Add(input[i].ToString());
                    }

                }
            }             

            //Console.WriteLine(string.Join("\n", output));
            return output.ToArray();
        }

        private static decimal Calculate(string[] input)
        {
            List<decimal> numbers = new List<decimal>();
            List<char> sings = new List<char>();

            //simplies and calculates *, / and ^
            string[] temp = MutiplyDivision(SimplifyInput(input));

            //seperate sings and numbers
            foreach (var c in temp)
            {
                if (decimal.TryParse(c, out decimal number))
                {
                    numbers.Add(number);
                }
                else
                {
                    sings.Add(char.Parse(c));
                }
            }

            decimal result = numbers[0];

            //calculation
            for (int i = 0; i < sings.Count; i++)
            {
                switch (sings[i])
                {
                    case '+':
                        result += numbers[i + 1];
                        break;
                    case '-':
                        result -= numbers[i + 1];
                        break;
                }
            }

            return result;
        }

        private static string[] SimplifyInput(string[] input)
        {
            if (!input.Contains("(") && !input.Contains(")"))
                return input;

            List<string> output = new List<string>();

            for(int i = 0; i <= input.Count(x => x == "("); i++)
            {
                output = new List<string>();

                //declare index of last ( and first )
                int indexStart = input.ToList().LastIndexOf("(");
                int indexFinish = input.ToList().IndexOf(")");

                //declare sub calculation
                List<string> temp = new List<string>();

                //add chars before (
                for (int j = 0; j < indexStart; j++)
                {
                    output.Add(input[j]);
                }

                //fill sub calculation
                for (int j = indexStart + 1; j < indexFinish; j++)
                    temp.Add(input[j]);

                //subcalculate and add to output
                decimal result = Calculate(temp.ToArray());
                output.Add(result.ToString());

                //add remaining chars
                for (int j = indexFinish + 1; j < input.Length; j++)
                {
                    output.Add(input[j]);
                }

                input = output.ToArray();
            }
            return output.ToArray();
        }

        private static string[] MutiplyDivision(string[] input)
        {
            List<string> output = input.ToList();

            while (output.Contains("*") || output.Contains("*"))
            {
                int index = output.IndexOf(output.FirstOrDefault(x => x == "*" || x == "/"));

                decimal n1 = decimal.Parse(output[index - 1]),
                    n2 = decimal.Parse(output[index + 1]),
                    result = 1;

                switch (output[index])
                {
                    case "*":
                        result = n1 * n2;
                        break;
                    case "/":
                        result = n1 / n2;
                        break;
                }

                //update the list
                output[index] = result.ToString();

                output.RemoveAt(index + 1);
                output.RemoveAt(index - 1);

            }

            return output.ToArray();
        }

        private static string[][] ValidateEquation(string[] input)
        {
            string[][] result = new string[2][];

            List<string> temp1 = new List<string>();
            List<string> temp2 = new List<string>();
            bool startWritingInTemp2 = false;

            foreach (var item in input)
            {
                if (item == "=")
                {
                    startWritingInTemp2 = true;
                    continue;
                }

                if (startWritingInTemp2)
                {
                    temp2.Add(item);
                }
                else
                {
                    temp1.Add(item);
                }
            }

            if (temp1.Any(x => x.All(a=> char.IsLetter(a))))
            {
                result[0] = temp1.ToArray();
                result[1] = temp2.ToArray();


            }
            else
            {
                result[1] = temp1.ToArray();
                result[0] = temp2.ToArray();
            }

            /*Console.WriteLine(String.Join(" ", temp1));
            Console.WriteLine(String.Join(" ", temp2));*/

            return result;
        }

        private static decimal EquationSolver(string[][] input)
        {
            decimal output = 0;

            List<string> temp = new List<string>();
            string[] withX = input[0];
            string[] withoutX = input[1];

            int Xindex = Array.IndexOf(withX, withX.First(x => x.All(a => char.IsLetter(a))));

            string Xsing;
            decimal RightResult = Calculate(withoutX);

            for (int i = 0; i < withX.Length; i++)
            {
                if (Xindex == i)
                {
                    continue;
                }

                temp.Add(withX[i]);
            }

            if (temp.Count == 0)
            {
                return RightResult;
            }            

            if (Xindex == 0)
            {
                Xsing = withX[Xindex + 1];
                temp.RemoveAt(Xindex);
            }
            else
            {
                Xsing = withX[Xindex - 1];
                temp.RemoveAt(Xindex - 1);
            }

            decimal LeftResult = Calculate(temp.ToArray());

            switch (Xsing)
            {
                case "+":
                    output = RightResult - LeftResult;
                    break;
                case "-":
                    output = LeftResult - RightResult;
                    break;
                case "*":
                    output = RightResult / LeftResult;
                    break;
                case "/":
                    output = LeftResult / RightResult;
                    break;
            }

            //Console.WriteLine(String.Join("", temp));
            return output;
        }

        private static bool IsSignCorrect(string c)
        {
            switch (c)
            {
                case "+":
                case "-":
                case "*":
                case "/":
                case "^":
                case "=":
                    return true;
                default:
                    return false;
            }
        }
    }
}
