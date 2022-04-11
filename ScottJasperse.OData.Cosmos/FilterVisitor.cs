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
                BinaryOperatorKind.And => "AND",
                BinaryOperatorKind.Or => "OR",
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

        public override QueryNode Visit(SingleValueFunctionCallNode nodeIn)
        {
            if (nodeIn.Name.ToLower() == "contains")
            {
                stringBuilder.Append($"CONTAINS(");
                nodeIn.Parameters.ElementAt(0).Accept(this);
                stringBuilder.Append(", ");
                nodeIn.Parameters.ElementAt(1).Accept(this);
                stringBuilder.Append(")");
            }
            else
            {
                throw new NotImplementedException($"Support for '{nodeIn.Name}' is not yet");
            }

            return nodeIn;
        }

        public override QueryNode Visit(ConvertNode nodeIn)
        {
            nodeIn.Source.Accept(this);
            return nodeIn;
        }

        public override string ToString()
        {
            return stringBuilder.ToString();
        }
    }

}