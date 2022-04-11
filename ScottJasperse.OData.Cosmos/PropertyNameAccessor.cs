using Microsoft.OData.UriParser;

namespace ScottJasperse.OData.Cosmos
{
    class PropertyNameAccessor
    {
        private readonly Func<string, string> translatePropertyName;

        public PropertyNameAccessor(Func<string, string> translatePropertyName)
        {
            this.translatePropertyName = translatePropertyName;
        }

        public string GetFullPropertyPath(SingleValuePropertyAccessNode node)
        {
            var heirarchy = new List<string>();
            heirarchy.AddRange(GetAncestors(node.Source));
            heirarchy.Add(translatePropertyName(node.Property.Name));

            return string.Join(".", heirarchy);
        }
        
        private IEnumerable<string> GetAncestors(SingleValueNode node)
        {
            var fieldStack = new List<string>();

            if (node is SingleNavigationNode navNode)
            {
                fieldStack.AddRange(GetAncestors(navNode.Source));
                fieldStack.Add(translatePropertyName(navNode.NavigationProperty.Name));
            }
            
            return fieldStack;
        }
    }
}