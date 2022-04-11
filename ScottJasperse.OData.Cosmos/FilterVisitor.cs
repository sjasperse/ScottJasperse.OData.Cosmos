using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.UriParser;

namespace ScottJasperse.OData.Cosmos
{
    class FilterVisitor : QueryNodeVisitor<QueryNode>
    {
        private readonly StringBuilder stringBuilder = new StringBuilder();
        private readonly Func<object, string> createParameter;
        private readonly Func<string, string> propertyNameTranslator;

        public FilterVisitor(Func<object, string> createParameter, Func<string, string> propertyNameTranslator)
        {
            this.createParameter = createParameter;
            this.propertyNameTranslator = propertyNameTranslator;

            stringBuilder.Append("WHERE ");
        }

        public override QueryNode Visit(BinaryOperatorNode nodeIn)
        {
            nodeIn.Left.Accept(this);
            stringBuilder.Append(" ");
            Visit(nodeIn.OperatorKind);
            stringBuilder.Append(" ");
            nodeIn.Right.Accept(this);

            return nodeIn;
        }

        private void Visit(BinaryOperatorKind op)
        {
            stringBuilder.Append(op switch {
                BinaryOperatorKind.Equal => "=",
                BinaryOperatorKind.GreaterThan => ">",
                BinaryOperatorKind.GreaterThanOrEqual => ">=",
                BinaryOperatorKind.LessThan => "<",
                BinaryOperatorKind.LessThanOrEqual => "<=",
                _ => throw new NotImplementedException($"BinaryOperatorKind.{op.ToString()} not implemented")
            });
        }

        public override QueryNode Visit(SingleValuePropertyAccessNode nodeIn)
        {
            var heirarchy = new List<string>();
            heirarchy.Add("c");
            heirarchy.AddRange(GetAncestors(nodeIn.Source));
            heirarchy.Add(propertyNameTranslator(nodeIn.Property.Name));

            stringBuilder.Append(string.Join(".", heirarchy));

            return nodeIn;
        }

        private IEnumerable<string> GetAncestors(SingleValueNode node)
        {
            var fieldStack = new List<string>();

            if (node is SingleNavigationNode navNode)
            {
                fieldStack.AddRange(GetAncestors(navNode.Source));
                fieldStack.Add(propertyNameTranslator(navNode.NavigationProperty.Name));
            }
            
            return fieldStack;
        }

        public override QueryNode Visit(ConstantNode nodeIn)
        {
            stringBuilder.Append(createParameter(nodeIn.Value));
            return nodeIn;
        }

        public override string ToString()
        {
            return stringBuilder.ToString();
        }
    }

}