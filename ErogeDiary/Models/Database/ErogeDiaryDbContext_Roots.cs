using System;
using System.Threading.Tasks;

namespace ErogeDiary.Models.Database;

public partial class ErogeDiaryDbContext
{
    public async Task AddRootAsync(RootData root)
    {
        Roots.Add(root);
        await SaveChangesAsync();
    }

    public async Task UpdateRootAsync(RootData root)
    {
        Roots.Update(root);
        await SaveChangesAsync();
    }

    public async Task RemoveRootAsync(RootData root)
    {
        Roots.Remove(root);
        await SaveChangesAsync();
    }
}
