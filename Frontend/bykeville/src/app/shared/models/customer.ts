export interface Customer {
    CustomerId?: number;
    NameStyle: boolean;
    Title?: string;
    FirstName: string;
    MiddleName?: string;
    LastName: string;
    Suffix?: string;
    CompanyName?: string;
    SalesPerson?: string;
    EmailAddress: string;
    Phone: string;
    PasswordHash: string;
    PasswordSalt?: string;
    Rowguid?: string;
    ModifiedDate?: string;
    CustomerAddresses: [];
    SalesOrderHeaders: [];
  }
  