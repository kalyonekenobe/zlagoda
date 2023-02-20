import {ReactNode} from "react";

export type LayoutProps = {
  children?: ReactNode | [ReactNode],
}

export type NavbarLinksType = {
  [name: string]: string,
}