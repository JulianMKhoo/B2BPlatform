"use client";

import { createContext, useContext, useState, useEffect, type ReactNode } from "react";

type Role = "owner" | "employee" | null;

const TWO_DAYS_MS = 2 * 24 * 60 * 60 * 1000;

interface AuthState {
  ownerId: number | null;
  companyId: number | null;
  employeeId: number | null;
  unitId: number | null;
  role: Role;
  employeeRole: number | null;
  email: string | null;
  firstName: string | null;
  lastName: string | null;
  loginAt: number | null;
}

interface AuthContextType extends AuthState {
  loginAsOwner: (ownerId: number, companyId: number) => void;
  loginAsEmployee: (employeeId: number, companyId: number, unitId: number, employeeRole: number, email: string, firstName: string, lastName: string) => void;
  updateUnitId: (newUnitId: number) => void;
  logout: () => void;
  isLoggedIn: boolean;
  isOwner: boolean;
  isEmployee: boolean;
  isAdmin: boolean;
  canManage: boolean;
  hydrated: boolean;
}

const emptyState: AuthState = {
  ownerId: null,
  companyId: null,
  employeeId: null,
  unitId: null,
  role: null,
  employeeRole: null,
  email: null,
  firstName: null,
  lastName: null,
  loginAt: null,
};

const AuthContext = createContext<AuthContextType | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [state, setState] = useState<AuthState>(emptyState);
  const [hydrated, setHydrated] = useState(false);

  useEffect(() => {
    const saved = localStorage.getItem("auth");
    if (saved) {
      const parsed: AuthState = JSON.parse(saved);
      if (parsed.loginAt && Date.now() - parsed.loginAt > TWO_DAYS_MS) {
        localStorage.removeItem("auth");
      } else {
        setState(parsed);
      }
    }
    setHydrated(true);
  }, []);

  const loginAsOwner = (ownerId: number, companyId: number) => {
    const s: AuthState = { ...emptyState, ownerId, companyId, role: "owner", loginAt: Date.now() };
    setState(s);
    localStorage.setItem("auth", JSON.stringify(s));
  };

  const loginAsEmployee = (employeeId: number, companyId: number, unitId: number, employeeRole: number, email: string, firstName: string, lastName: string) => {
    const s: AuthState = { ...emptyState, employeeId, companyId, unitId, role: "employee", employeeRole, email, firstName, lastName, loginAt: Date.now() };
    setState(s);
    localStorage.setItem("auth", JSON.stringify(s));
  };

  const updateUnitId = (newUnitId: number) => {
    setState((prev) => {
      const s = { ...prev, unitId: newUnitId };
      localStorage.setItem("auth", JSON.stringify(s));
      return s;
    });
  };

  const logout = () => {
    setState(emptyState);
    localStorage.removeItem("auth");
  };

  const isLoggedIn = state.role === "owner" ? !!state.ownerId : state.role === "employee" ? !!state.employeeId : false;
  const isOwner = state.role === "owner";
  const isEmployee = state.role === "employee";
  const isAdmin = isEmployee && state.employeeRole === 1;
  const canManage = isOwner || isAdmin;

  return (
    <AuthContext.Provider
      value={{
        ...state,
        loginAsOwner,
        loginAsEmployee,
        updateUnitId,
        logout,
        isLoggedIn,
        isOwner,
        isEmployee,
        isAdmin,
        canManage,
        hydrated,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be inside AuthProvider");
  return ctx;
}
