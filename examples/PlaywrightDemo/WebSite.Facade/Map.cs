using Caravel.Abstractions;
using WebSite.Facade.POMs.Abstractions;
using WebSite.Facade.POMs.Pages;

namespace WebSite.Facade;

public sealed class Map
{
    private readonly ICollection<IPOM> _poms;

    public Map(ICollection<IPOM> poms)
    {
        _poms = poms;
    }

    public PageA PageA => GetPOM<PageA>();
    public PageB PageB => GetPOM<PageB>();
    public PageC PageC => GetPOM<PageC>();
    public PageD PageD => GetPOM<PageD>();
    public PageE PageE => GetPOM<PageE>();

    public T GetPOM<T>()
        where T : INode => (T)_poms.Where(x => x.GetType() == typeof(T)).Single();
}
