using Auth.Infrastracture.Messages.Queries;
using Auth.Query.Dtos.AppGroups;


namespace Auth.Query.Queries.AppGroups
{
    public class GetAllAppGroupQuery : IQuery<List<AppGroupSimpleDto>>
    {
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
    }

}
