using System;
using System.Collections.Generic;

namespace fitness.Models;

public partial class Abonement
{
    public int IdAbonement { get; set; }

    public string AbonementName { get; set; } = null!;

    public int AbonementPrice { get; set; }

    public int AbonementLong { get; set; }

    public string AbonementDescription { get; set; } = null!;

    public int? AbonementFreeze { get; set; }

    public virtual ICollection<AbonementClient> AbonementClients { get; set; } = new List<AbonementClient>();
}
