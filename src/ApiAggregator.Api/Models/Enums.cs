namespace ApiAggregator.Api.Models;

public enum SourceStatus { Live, Cached, Stale, Failed }
public enum SortBy { Date, Score, Title }
public enum SortOrder { Desc, Asc }