# ScottJasperse.OData.Cosmos

Project to demonstrate building simple queries for Cosmos from OData query parameters.

Is not intended to support *everything* - just the most common scenarios.

See the [Tests](ScottJasperse.OData.Api.Tests/Tests.cs) for an idea of what is supported.

Supported:
- `$select` by top-level properties
- `$filter` by top level, or child properties. 
- `$filter=contains(....)`
- `$orderby` by top level, or child properties. Asending or Descending.
- `$top` via cosmos `MaxItemsCount` property on the `QueryRequestOptions`.