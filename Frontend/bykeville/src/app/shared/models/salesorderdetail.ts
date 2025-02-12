import { Product } from "./product";

export interface SalesOrderDetail {
    salesOrderId: number;
    salesOrderDetailId: number;
    orderQty: number;
    productId: number;
    unitPrice: number;
    unitPriceDiscount: number;
    lineTotal: number;
    rowguid: string;
    modifiedDate: string;
    product?: Product | null;
    salesOrder?: {} | null;
  }