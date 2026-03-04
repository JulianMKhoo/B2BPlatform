"use client";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { useAuth } from "@/lib/auth-context";
import { employees } from "@/lib/api";
import { useEffect, useState } from "react";

const fullNav = [
  { href: "/dashboard", label: "Overview" },
  { href: "/dashboard/employees", label: "Employees" },
  { href: "/dashboard/business-units", label: "Business Units" },
  { href: "/dashboard/chat", label: "Chat" },
];

const employeeNav = [
  { href: "/dashboard/chat", label: "Chat" },
];

export default function DashboardLayout({ children }: { children: React.ReactNode }) {
  const pathname = usePathname();
  const router = useRouter();
  const { isLoggedIn, isOwner, isEmployee, isAdmin, canManage, ownerId, employeeId, unitId, firstName, lastName, logout, updateUnitId, hydrated } = useAuth();
  const [noBuChecked, setNoBuChecked] = useState(false);

  useEffect(() => {
    if (hydrated && !isLoggedIn) router.push("/auth/login");
  }, [hydrated, isLoggedIn, router]);

  useEffect(() => {
    if (!canManage && pathname === "/dashboard") {
      router.push("/dashboard/chat");
    }
  }, [canManage, pathname, router]);

  // Auto-check if employee's BU has been reassigned
  useEffect(() => {
    if (!hydrated || !isEmployee || !employeeId) return;
    if (unitId && unitId > 0) { setNoBuChecked(true); return; }
    employees.checkUnit(employeeId).then((res) => {
      if (res.unitId && res.unitId > 0) {
        updateUnitId(res.unitId);
      }
      setNoBuChecked(true);
    });
  }, [hydrated, isEmployee, employeeId, unitId]); // eslint-disable-line react-hooks/exhaustive-deps

  if (!hydrated || !isLoggedIn) return null;

  // Show "No BU" state for employees with deleted/missing BU
  if (isEmployee && noBuChecked && (!unitId || unitId === 0)) {
    return (
      <div className="min-h-screen bg-background">
        <header className="bg-card border-b border-border">
          <div className="max-w-7xl mx-auto px-6 h-14 flex items-center justify-between">
            <span className="font-bold text-lg">B2B Platform</span>
            <button
              onClick={() => { logout(); router.push("/auth/login"); }}
              className="text-sm text-danger hover:underline"
            >
              Logout
            </button>
          </div>
        </header>
        <main className="max-w-7xl mx-auto px-6 py-8">
          <div className="bg-yellow-50 border border-yellow-200 rounded-xl p-8 text-center">
            <h2 className="text-xl font-bold text-yellow-800 mb-3">No Business Unit</h2>
            <p className="text-yellow-700">
              Your business unit has been removed. Please wait for your company owner to reassign you, then refresh this page.
            </p>
          </div>
        </main>
      </div>
    );
  }

  const nav = canManage ? fullNav : employeeNav;
  const employeeName = firstName && lastName ? `${firstName} ${lastName}` : `#${employeeId}`;
  const label = isOwner
    ? `Owner #${ownerId}`
    : isAdmin
      ? `Admin: ${employeeName}`
      : `Employee: ${employeeName}`;

  return (
    <div className="min-h-screen bg-background">
      <header className="bg-card border-b border-border">
        <div className="max-w-7xl mx-auto px-6 h-14 flex items-center justify-between">
          <div className="flex items-center gap-8">
            <span className="font-bold text-lg">B2B Platform</span>
            <nav className="flex gap-1">
              {nav.map((n) => (
                <Link
                  key={n.href}
                  href={n.href}
                  className={`px-3 py-1.5 rounded-md text-sm transition-colors ${
                    pathname === n.href
                      ? "bg-primary text-white"
                      : "text-muted hover:text-foreground hover:bg-gray-100"
                  }`}
                >
                  {n.label}
                </Link>
              ))}
            </nav>
          </div>
          <div className="flex items-center gap-4">
            <Link href="/dashboard/profile" className="text-sm text-muted hover:text-foreground hover:underline">
              {label}
            </Link>
            <button
              onClick={() => { logout(); router.push("/auth/login"); }}
              className="text-sm text-danger hover:underline"
            >
              Logout
            </button>
          </div>
        </div>
      </header>
      <main className="max-w-7xl mx-auto px-6 py-8">{children}</main>
    </div>
  );
}
