namespace Bitsie.Shop.Services
{
    public class MapperService : IMapperService
    {
        public TDest Map<TSrc, TDest>(TSrc source, TDest dest) where TDest : class
        {
            return AutoMapper.Mapper.Map(source, dest);
        }
    }
}
