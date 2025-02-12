export interface SalesOrderHeader {
    salesOrderId: number;
    revisionNumber: number;
    orderDate: string;
    dueDate: string;
    shipDate?: string | null;
    status: number;
    onlineOrderFlag: boolean;
    salesOrderNumber: string;
    purchaseOrderNumber?: string | null;
    accountNumber?: string | null;
    customerId: number;
    shipToAddressId?: number | null;
    billToAddressId?: number | null;
    shipMethod: string;
    creditCardApprovalCode?: string | null;
    subTotal: number;
    taxAmt: number;
    freight: number;
    totalDue: number;
    comment?: string | null;
    rowguid: string;
    modifiedDate: string;
    billToAddress?: {} | null;
    customer: {} | null;
    salesOrderDetails: [];
    shipToAddress?: {} | null;
  }