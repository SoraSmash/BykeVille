export interface NewCustomer {
  customerId: number;
  nameStyle: boolean;
  title?: string;
  firstName: string;
  middleName?: string;
  lastName: string;
  suffix?: string;
  companyName?: string;
  salesPerson?: string;
  emailAddress?: string;
  phone?: string;
  passwordHash: string;
  passwordSalt: string;
  rowguid: string;
  modifiedDate: string;
  role: string;
  customerOldId: number;
  newCustomerAddresses: [];
}