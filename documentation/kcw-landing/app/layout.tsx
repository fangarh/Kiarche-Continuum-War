import type { Metadata } from "next";
import { ReactNode } from "react";

import "./globals.css";

export const metadata: Metadata = {
    title: "Ки'Архе: Война Континуума | Лендинг",
    description:
        "Кинематографичный лендинг игры «Ки'Архе: Война Континуума» о войне за древнюю сеть порталов, героях, фракциях и мирах галактики.",
};

type RootLayoutProps = Readonly<{
    children: ReactNode;
}>;

export default function RootLayout({ children }: RootLayoutProps) {
    return (
        <html lang="ru">
            <body>{children}</body>
        </html>
    );
}
