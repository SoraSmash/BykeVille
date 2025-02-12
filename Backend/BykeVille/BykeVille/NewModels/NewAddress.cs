using System;
using System.Collections.Generic;

namespace BykeVille.NewModels;

/// <summary>
/// Street address information for customers.
/// </summary>
public partial class NewAddress
{
    /// <summary>
    /// Primary key for Address records.
    /// </summary>
    public int AddressId { get; set; }

    /// <summary>
    /// First street address line.
    /// </summary>
    public string AddressLine1 { get; set; } = null!;

    /// <summary>
    /// Second street address line.
    /// </summary>
    public string? AddressLine2 { get; set; }

    /// <summary>
    /// Name of the city.
    /// </summary>
    public string City { get; set; } = null!;

    /// <summary>
    /// Name of state or province.
    /// </summary>
    public string StateProvince { get; set; } = null!;

    public string CountryRegion { get; set; } = null!;

    /// <summary>
    /// Postal code for the street address.
    /// </summary>
    public string PostalCode { get; set; } = null!;

    /// <summary>
    /// ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.
    /// </summary>
    public Guid Rowguid { get; set; }

    /// <summary>
    /// Date and time the record was last updated.
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    public int AddressOldId { get; set; }

    public virtual ICollection<NewCustomerAddress> NewCustomerAddresses { get; set; } = new List<NewCustomerAddress>();

    public NewAddress(int addressId, string addressLine1, string? addressLine2, string city, string stateProvince, string countryRegion, string postalCode, Guid rowguid, DateTime modifiedDate, int addressOldId)
    {
        AddressId = addressId;
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        City = city;
        StateProvince = stateProvince;
        CountryRegion = countryRegion;
        PostalCode = postalCode;
        Rowguid = rowguid;
        ModifiedDate = modifiedDate;
        AddressOldId = addressOldId;
    }
}
