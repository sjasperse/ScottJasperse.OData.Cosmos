using System.Text;
using Microsoft.OData.UriParser;

namespace ScottJasperse.OData.Cosmos
{
    class FilterVisitor : QueryNodeVisitor<QueryNode>
    {
        private readonly StringBuilder stringBuilder = new StringBuilder();
        private readonly Func<object, string> createParameter;
        private readonly PropertyNameAccessor propertyNameAccessor;

        public FilterVisitor(Func<object, string> createParameter, PropertyNameAccessor propertyNameAccessor)
        {
            this.createParameter = createParameter;
            this.propertyNameAccessor = propertyNameAccessor;

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
            stringBuilder.Append("c." + propertyNameAccessor.GetFullPropertyPath(nodeIn));

            return nodeIn;
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