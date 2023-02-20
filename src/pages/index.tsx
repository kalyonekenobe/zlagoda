import {Layout} from "@/components/Layout";
import Head from 'next/head';
import {HomepageHeader} from "@/components/HomepageHeader";

const Home = () => (
  <Layout>
    <Head>
      <title>Zlagoda - Homepage</title>
    </Head>
    <HomepageHeader />
  </Layout>
)

export default Home;
