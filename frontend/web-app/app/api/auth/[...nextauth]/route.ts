import NextAuth, { NextAuthOptions } from "next-auth"
import DuendeIDS6Provider from "next-auth/providers/duende-identity-server6"
import { authOptions } from "./authOptions/authOptions";

const handler = NextAuth(authOptions);

export { handler as GET, handler as POST }