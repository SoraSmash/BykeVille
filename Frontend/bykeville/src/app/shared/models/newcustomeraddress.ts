import { NewAddress } from "./newaddress";
import { NewCustomer } from "./newcustomer";

export interface NewCustomerAddress {
    customerId: number;
    addressId: number;
    addressType: string;
    rowguid: string;
    modifiedDate: string;
    customerOldId: number;
    addressOldId: number;
    address: NewAddress;
    customer: NewCustomer;
  }