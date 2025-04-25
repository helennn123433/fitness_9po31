using System;
using System.Collections.Generic;

namespace fitness.Models;

public partial class Client
{
    public int IdClient { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<AbonementClient> AbonementClients { get; set; } = new List<AbonementClient>();

    public virtual User? User { get; set; }
}
