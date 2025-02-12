export interface NewAddress {
    addressId: number;
    addressLine1: string;
    addressLine2?: string | null;
    city: string;
    stateProvince: string;
    countryRegion: string;
    postalCode: string;
    rowguid: string;
    modifiedDate: string;
    addressOldId: number;
    newCustomerAddresses: [];
  }
  