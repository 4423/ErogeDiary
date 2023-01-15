using ErogeDiary.Models.Database.Entities;
using System;
using System.Threading.Tasks;

namespace ErogeDiary.Models.Database;

public partial class ErogeDiaryDbContext
{
    public async Task AddRootAsync(Root root)
    {
        Roots.Add(root);
        await SaveChangesAsync();
    }

    public async Task UpdateRootAsync(Root root)
    {
        Roots.Update(root);
        await SaveChangesAsync();
    }

    public async Task RemoveRootAsync(Root root)
    {
        Roots.Remove(root);
        await SaveChangesAsync();
    }
}
