﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace AtlasERP.Models;

public partial class AsignRepuRequer
{
    public int Id { get; set; }

    public string Codrep { get; set; }

    public int? IdRequer { get; set; }

    public DateTime Fecrea { get; set; }

    public int? Estado { get; set; }

    public string Usercrea { get; set; }

    public string Codcia { get; set; }

    public int? Cantidad { get; set; }

    public decimal? ValorFinal { get; set; }
}