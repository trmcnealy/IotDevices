using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using RaspberryPiDevices;

namespace RPiDevices;

public sealed class RPiServices
{
    private readonly RPiSettings _settings;

    public RPiServices(IOptionsSnapshot<RPiSettings> namedOptionsAccessor)
    {
        _settings = namedOptionsAccessor.Get("RPiSettings");
    }
}
