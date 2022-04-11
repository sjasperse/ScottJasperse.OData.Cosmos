using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.UriParser;

namespace ScottJasperse.OData.Cosmos
{
    class DiagnosticVisitor : QueryNodeVisitor<QueryNode>
    {
        private readonly string prefix;
        private readonly StringWriter writer;

        public DiagnosticVisitor(StringWriter writer, string prefix = "")
        {
            this.prefix = prefix;
            this.writer = writer;
        }

        private DiagnosticVisitor GetNestedVisitor(string prefix)
            => new DiagnosticVisitor(writer, prefix);

        private void WriteLine(string line)
            => writer.WriteLine(prefix + line);

        public override QueryNode Visit(AllNode nodeIn)
        {
            WriteLine("AllNode");
            return nodeIn;
        }

        public override QueryNode Visit(AnyNode nodeIn)
        {
            WriteLine("AnyNode");
            return nodeIn;
        }

        public override QueryNode Visit(BinaryOperatorNode nodeIn)
        {
            WriteLine("BinaryOperatorNode");
            WriteLine("Left:");
            nodeIn.Left.Accept(GetNestedVisitor("  "));
            WriteLine($"OperatorKind: {nodeIn.OperatorKind.ToString()}");
            WriteLine("Right:");
            nodeIn.Right.Accept(GetNestedVisitor("  "));
            return nodeIn;
        }

        public override QueryNode Visit(CountNode nodeIn)
        {
            WriteLine("CountNode");
            return nodeIn;
        }

        public override QueryNode Visit(CollectionNavigationNode nodeIn)
        {
            WriteLine("CollectionNavigationNode");
            return nodeIn;
        }

        public override QueryNode Visit(CollectionPropertyAccessNode nodeIn)
        {
            WriteLine("CollectionPropertyAccessNode");
            return nodeIn;
        }

        public override QueryNode Visit(CollectionOpenPropertyAccessNode nodeIn)
        {
            WriteLine("CollectionOpenPropertyAccessNode");
            return nodeIn;
        }

        public override QueryNode Visit(ConstantNode nodeIn)
        {
            WriteLine("ConstantNode");
            return nodeIn;
        }

        public override QueryNode Visit(CollectionConstantNode nodeIn)
        {
            WriteLine("CollectionConstantNode");
            return nodeIn;
        }

        public override QueryNode Visit(ConvertNode nodeIn)
        {
            WriteLine("ConvertNode");
            return nodeIn;
        }

        public override QueryNode Visit(CollectionResourceCastNode nodeIn)
        {
            WriteLine("CollectionResourceCastNode");
            return nodeIn;
        }

        public override QueryNode Visit(ResourceRangeVariableReferenceNode nodeIn)
        {
            WriteLine("ResourceRangeVariableReferenceNode");
            return nodeIn;
        }

        public override QueryNode Visit(NonResourceRangeVariableReferenceNode nodeIn)
        {
            WriteLine("NonResourceRangeVariableReferenceNode");
            return nodeIn;
        }

        public override QueryNode Visit(SingleResourceCastNode nodeIn)
        {
            WriteLine("SingleResourceCastNode");
            return nodeIn;
        }

        public override QueryNode Visit(SingleNavigationNode nodeIn)
        {
            WriteLine("SingleNavigationNode");
            return nodeIn;
        }

        public override QueryNode Visit(SingleResourceFunctionCallNode nodeIn)
        {
            WriteLine("SingleResourceFunctionCallNode");
            return nodeIn;
        }

        public override QueryNode Visit(SingleValueFunctionCallNode nodeIn)
        {
            WriteLine("SingleValueFunctionCallNode");
            return nodeIn;
        }

        public override QueryNode Visit(CollectionResourceFunctionCallNode nodeIn)
        {
            WriteLine("CollectionResourceFunctionCallNode");
            return nodeIn;
        }

        public override QueryNode Visit(CollectionFunctionCallNode nodeIn)
        {
            WriteLine("CollectionFunctionCallNode");
            return nodeIn;
        }

        public override QueryNode Visit(SingleValueOpenPropertyAccessNode nodeIn)
        {
            WriteLine("SingleValueOpenPropertyAccessNode");
            return nodeIn;
        }

        public override QueryNode Visit(SingleValuePropertyAccessNode nodeIn)
        {
            WriteLine("SingleValuePropertyAccessNode");
            WriteLine($"Property.Name: {nodeIn.Property.Name}");

            return nodeIn;
        }

        public override QueryNode Visit(UnaryOperatorNode nodeIn)
        {
            WriteLine("UnaryOperatorNode");
            return nodeIn;
        }

        public override QueryNode Visit(NamedFunctionParameterNode nodeIn)
        {
            WriteLine("NamedFunctionParameterNode");
            return nodeIn;
        }

        public override QueryNode Visit(ParameterAliasNode nodeIn)
        {
            WriteLine("ParameterAliasNode");
            return nodeIn;
        }

        public override QueryNode Visit(SearchTermNode nodeIn)
        {
            WriteLine("SearchTermNode");
            return nodeIn;
        }

        public override QueryNode Visit(SingleComplexNode nodeIn)
        {
            WriteLine("SingleComplexNode");
            return nodeIn;
        }

        public override QueryNode Visit(CollectionComplexNode nodeIn)
        {
            WriteLine("CollectionComplexNode");
            return nodeIn;
        }

        public override QueryNode Visit(SingleValueCastNode nodeIn)
        {
            WriteLine("SingleValueCastNode");
            return nodeIn;
        }

        public override QueryNode Visit(AggregatedCollectionPropertyNode nodeIn)
        {
            WriteLine("AggregatedCollectionPropertyNode");
            return nodeIn;
        }

        public override QueryNode Visit(InNode nodeIn)
        {
            WriteLine("InNode");
            return nodeIn;
        }
    }
}