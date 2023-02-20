import {FC} from "react";
import {Navbar} from "@/components/Navbar";
import Image from 'next/image';
import groceryBasket from 'public/grocery_basket.jpg';

export const HomepageHeader: FC = () => {


  //className={`bg-[url('../../public/grocery.jpg')] bg-cover h-screen`}
  return (
    <header>
      <Navbar />
      <div className="header-content max-w-screen-2xl mx-auto">
        <Image src={groceryBasket.src} alt={"Grocery Basket"} width={500} height={450} />
      </div>
    </header>
  )
}