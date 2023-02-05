using System;
using System.Collections.Generic;
using System.Linq;

namespace Exam.Expressionist
{
    public class Expressionist : IExpressionist
    {
        Dictionary<string, Expression> expressionsById = new Dictionary<string, Expression>();
        Dictionary<string, Dictionary<string, Expression>> expressionsByParentId = new Dictionary<string, Dictionary<string, Expression>>();
        Dictionary<string, string> parentsByChildId = new Dictionary<string, string>();

        public void AddExpression(Expression expression)
        {
            if (expressionsById.Count != 0)
            {
                throw new ArgumentException();
            }

            expressionsById.Add(expression.Id, expression);
            expressionsByParentId.Add(expression.Id, new Dictionary<string, Expression>());
        }

        public void AddExpression(Expression expression, string parentId)
        {
            if (!expressionsById.ContainsKey(parentId))
            {
                throw new ArgumentException();
            }
            if (expressionsById[parentId].LeftChild != null && expressionsById[parentId].RightChild != null)
            {
                throw new ArgumentException();
            }

            if (expressionsById[parentId].LeftChild is null)
            {
                expressionsById[parentId].LeftChild = expression;
                expression.Parent = expressionsById[parentId];

                expressionsByParentId[parentId].Add(expression.Id, expression);
                parentsByChildId.Add(expression.Id, parentId);
            }
            else if(expressionsById[parentId].RightChild is null)
            {
                expressionsById[parentId].RightChild = expression;
                expression.Parent = expressionsById[parentId];

                expressionsByParentId[parentId].Add(expression.Id, expression);
                parentsByChildId.Add(expression.Id, parentId);
            }
        }

        public bool Contains(Expression expression)
            => expressionsById.ContainsKey(expression.Id);

        public int Count()
            => expressionsById.Count;

        private void recursiveInorder(Expression root, string result)
        {
            if (root.LeftChild != null)
            {
                recursiveInorder(root.LeftChild, result);
            }

            if (root.Type == ExpressionType.Value)
            {
                result += root.Type.ToString();
            }
            else
            {
                result += "(" + root.LeftChild.Type.ToString() + root.Value.ToString() + root.RightChild.Value.ToString() + ")";
            }

            if (root.RightChild != null)
            {
                recursiveInorder(root.RightChild, result);
            }
        }

        public string Evaluate()
        {
            var result = "";

            var first = expressionsById.Values.First(e => e.Parent == null);
            recursiveInorder(first, result);

            return result;
        }

        public Expression GetExpression(string expressionId)
        {
            if (!expressionsById.ContainsKey(expressionId))
            {
                throw new ArgumentException();
            }

            return expressionsById[expressionId];
        }

        public void RemoveExpression(string expressionId)
        {
            if (!expressionsById.ContainsKey(expressionId))
            {
                throw new ArgumentException();
            }

            var expression = expressionsById[expressionId];
            if (expression.Parent is null)
            {
                expressionsById.Remove(expressionId);
            }
            else
            {
                var parent = expression.Parent;
                if (parent.LeftChild.Equals(expression))
                {
                    var rightChild = parent.RightChild;
                    parent.RightChild = null;
                    parent.LeftChild = rightChild;
                    expressionsById.Remove(expressionId);
                }
                else
                {
                    parent.RightChild = null;
                    expressionsById.Remove(expressionId);
                }
            }
        }
    }
}
