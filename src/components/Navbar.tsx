import {FC} from "react";
import {NavbarLinksType} from "@/types/components.types";
import Link from "next/link";
import {v4 as uuid} from "uuid";

let navbarLinks: NavbarLinksType = {
  Products: '/products',
  About: '/about',
}

export const Navbar: FC = () => (
  <nav className="navbar flex items-center w-full max-w-screen-2xl mx-auto justify-between p-2 mt-2 rounded-md bg-opacity-50 backdrop-blur-2xl">
    <div className="links flex items-center">
      <Link href={'/'} className="font-bold text-3xl mr-10 hover:text-blue-600">Zlagoda</Link>
      <div className="navbar-links flex items-center space-x-5">
        {
          Object.entries(navbarLinks).map(([name, url]) => (
            <Link href={url} key={uuid()} className={"hover:text-blue-600"}>{name}</Link>
          ))
        }
      </div>
    </div>
    <div className="auth-buttons flex items-center space-x-2">
      <Link href={'sign-in'} className="rounded-md px-2.5 py-1.5 text-sm bg-black text-white">Sign In</Link>
      <Link href={'sign-up'} className="rounded-md px-2.5 py-1.5 text-sm bg-orange-400">Sign Up</Link>
    </div>
  </nav>
)