﻿using Homemap.ApplicationCore.Interfaces.Repositories;
using Homemap.Domain.Core;
using Homemap.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Homemap.Infrastructure.Data.Repositories
{

    internal class DeviceRepository : CrudRepository<Device>, IDeviceRepository
    {
        private readonly ApplicationDbContext _context;

        public DeviceRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Device>> FindAllByReceiverId(int receiverId)
        {
            return await _context.Devices
                .Where(e => e.ReceiverId == receiverId)
                .ToListAsync();
        }
    }
}
