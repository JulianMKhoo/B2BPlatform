"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/lib/auth-context";
import Link from "next/link";

const cards = [
  {
    href: "/dashboard/employees",
    title: "Employees",
    desc: "Manage staff profiles and roles",
  },
  {
    href: "/dashboard/business-units",
    title: "Business Units",
    desc: "Create and manage BUs",
  },
  { href: "/dashboard/chat", title: "Chat", desc: "Manage workspace access" },
];

export default function DashboardPage() {
  const { ownerId, employeeId, companyId, isOwner, canManage } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (!canManage) router.push("/dashboard/chat");
  }, [canManage, router]);

  if (!canManage) return null;

  const displayId = isOwner ? `Owner #${ownerId}` : `Admin #${employeeId}`;

  return (
    <div>
      <h1 className="text-2xl font-bold mb-1">Dashboard</h1>
      <p className="text-muted mb-8">
        Welcome back ({displayId}, Company #{companyId})
      </p>

      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
        {cards.map((c) => (
          <Link
            key={c.href}
            href={c.href}
            className="bg-card border border-border rounded-xl p-6 hover:border-primary transition-colors"
          >
            <h2 className="font-semibold text-lg mb-1">{c.title}</h2>
            <p className="text-sm text-muted">{c.desc}</p>
          </Link>
        ))}
      </div>
    </div>
  );
}
