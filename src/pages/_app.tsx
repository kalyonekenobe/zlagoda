import '@/styles/globals.css'
import type { AppProps } from 'next/app'
import {Inter} from "@next/font/google";

const inter = Inter({
  subsets: ['latin', 'cyrillic'],
  variable: '--font-inter',
});

const App = ({ Component, pageProps }: AppProps) => (
  <div className={`${inter.variable} font-sans`}>
    <Component {...pageProps} />
  </div>
)

export default App;
