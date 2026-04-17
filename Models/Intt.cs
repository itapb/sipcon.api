using Models;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    
public class Intt : Record
{

    [SwaggerIgnore]
    public Int32 Total { get; set; } = 0;

    public string? MovementType { get; set; } = string.Empty;

    public string? CertificateNumber { get; set; } = string.Empty;

    public string? DealerRif { get; set; } = string.Empty;

    public string? ClientRif { get; set; } = string.Empty;

    public string? FirstName { get; set; } = string.Empty;

    public string? SecondName { get; set; } = string.Empty;

    public string? FirstLastName { get; set; } = string.Empty;

    public string? SecondLastName { get; set; } = string.Empty;

    public string? Address { get; set; } = string.Empty;

    public string? Urbanization { get; set; } = string.Empty;

    public string? House { get; set; } = string.Empty;

    public string? Floor { get; set; } = string.Empty;

    public string? Apartment { get; set; } = string.Empty;

    public string? Municipality { get; set; } = string.Empty;

    public string? Phone1Code { get; set; } = string.Empty;

    public string? Phone1Number { get; set; } = string.Empty;

    public string? Phone2Code { get; set; } = string.Empty;

    public string? Phone2Number { get; set; } = string.Empty;

    public string? Plate { get; set; } = string.Empty;

    public string? Brand { get; set; } = string.Empty;

    public string? State { get; set; } = string.Empty;

    public string? Vin { get; set; } = string.Empty;

    public string? InvoiceDate { get; set; } = string.Empty;

    public string? InvoiceNumber { get; set; } = string.Empty;

    [SwaggerIgnore]
    public string? NumberTxt { get; set; } = string.Empty;
    public string? Datos { get; set; } = string.Empty;
    }

}