using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RecursiveBinaryExpression
{
    class Program
    {
        static void Main(string[] args)
        {
            var myLemon = 5;
            var myOrange = 6;
            var myGrapefruit = 7;
            var myJerry = "blah";
            var myChris = "someVar";
            Expression<Func<MyClass, bool>> p = a => a.lemon == myLemon && a.oranges == myOrange && a.grapefruit == myGrapefruit;

            var myEsbPredicateClass = new MyClass();
            foreach (var element in GetBinaryExpression((BinaryExpression) p.Body))
            {
                // reflectively set property of myEsbPredicateClass using BinaryExpression
                Type type = myEsbPredicateClass.GetType();

                // get propValue
                PropertyInfo prop = type.GetProperty(GivePropName(element));

                // set propValue
                prop.SetValue(myEsbPredicateClass, GivePropValue(element), null);
            }
        }


        public static IEnumerable<BinaryExpression> GetBinaryExpression(BinaryExpression body)
        {
            var left = body.Left as BinaryExpression;
            if (left != null)
            {
                foreach (var binaryExpression in GetBinaryExpression(left))
                {
                    yield return binaryExpression;
                }
                yield return (BinaryExpression)body.Right;
            }
            else
            {
                yield return body;
            }
        }

        public static string GivePropName(BinaryExpression binaryExp)
        {
            var lhs = (MemberExpression)binaryExp.Left;
            return lhs.Member.Name;
        }

        public static object GivePropValue(BinaryExpression binaryExp)
        {
            var rhs = (MemberExpression)binaryExp.Right;
            var obj = ((ConstantExpression)rhs.Expression).Value;
            var field = (FieldInfo)rhs.Member;
            return field.GetValue(obj);
        }
    }

    public class MyClass
    {
        public int lemon { get; set; }
        public int oranges { get; set; }
        public int grapefruit { get; set; }
        public string jerry { get; set; }
        public string chris { get; set; }
    }
}
