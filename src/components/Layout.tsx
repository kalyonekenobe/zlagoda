import {FC} from "react";
import {LayoutProps} from "@/types/components.types";

export const Layout: FC<LayoutProps> = ({ children }) => (
  <div className="layout">
    {children}
  </div>
)