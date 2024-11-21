using SourceKit.Generators.Builder.Annotations;

namespace Lab5.Tools.Application.Abstractions.Persistence.Queries;

[GenerateBuilder]
public partial record OrderQuery(long[] OrderIds, [RequiredValue] int PageSize, long Cursor);