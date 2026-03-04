"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/lib/auth-context";
import { employees, businessUnits } from "@/lib/api";
import type { Employee, BusinessUnitItem } from "@/lib/types";
import { POSITION_LABELS, ROLE_LABELS } from "@/lib/types";

export default function EmployeesPage() {
  const { companyId, isOwner, isAdmin, canManage, employeeId } = useAuth();
  const router = useRouter();
  const [list, setList] = useState<Employee[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState({
    firstName: "",
    lastName: "",
    position: 1,
    email: "",
    unitId: "",
    role: 0,
  });
  const [submitting, setSubmitting] = useState(false);
  const [created, setCreated] = useState<{ employeeId: number; generatedPassword: string } | null>(null);
  const [buList, setBuList] = useState<BusinessUnitItem[]>([]);

  useEffect(() => {
    if (!canManage) router.push("/dashboard/chat");
  }, [canManage, router]);

  useEffect(() => {
    if (!companyId) return;
    businessUnits.list(companyId).then((res) => {
      const list = res.businessUnits || [];
      setBuList(list);
      if (list.length > 0) {
        setForm((f) => f.unitId === "" ? { ...f, unitId: String(list[0].id) } : f);
      }
    });
  }, [companyId]);

  const load = async () => {
    if (!companyId) return;
    setLoading(true);
    const res = (await employees.list(companyId)) as { employee?: Employee[] };
    setList(res.employee || []);
    setLoading(false);
  };

  useEffect(() => {
    load();
  }, [companyId]); //eslint-disable-line react-hooks/exhaustive-deps

  const handleAdd = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    setCreated(null);
    const res = await employees.createFull({
      firstName: form.firstName,
      lastName: form.lastName,
      position: form.position,
      email: form.email,
      unitId: +form.unitId,
      companyId: companyId!,
      role: form.role,
    });
    if (res.status?.code === "200") {
      setCreated({ employeeId: res.employeeId, generatedPassword: res.generatedPassword });
      setForm({ firstName: "", lastName: "", position: 1, email: "", unitId: "", role: 0 });
      load();
    }
    setSubmitting(false);
  };

  const handleDelete = async (id: number) => {
    await employees.delete(id);
    load();
  };

  const handleRoleChange = async (emp: Employee, newRole: number) => {
    if (!emp.employeeDataId) return;
    await employees.updateRole(emp.employeeDataId, newRole);
    load();
  };

  const handleUnitChange = async (emp: Employee, newUnitId: number) => {
    if (!emp.employeeDataId) return;
    await employees.updateUnit(emp.employeeDataId, newUnitId);
    load();
  };

  const canDeleteEmployee = (emp: Employee): boolean => {
    // Cannot delete self
    if (emp.id === employeeId) return false;
    // Owner can delete anyone
    if (isOwner) return true;
    // Admin can only delete regular employees (role 0)
    if (isAdmin) return emp.role === 0;
    return false;
  };

  if (!canManage) return null;

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold">Employees</h1>
        <button
          onClick={() => { setShowForm(!showForm); setCreated(null); }}
          className="bg-primary text-white px-4 py-2 rounded-lg text-sm hover:bg-primary-hover transition-colors"
        >
          {showForm ? "Cancel" : "Add Employee"}
        </button>
      </div>

      {showForm && (
        <form
          onSubmit={handleAdd}
          className="bg-card border border-border rounded-xl p-4 mb-6 space-y-3"
        >
          <div className="flex gap-3">
            <div className="flex-1">
              <label className="block text-xs font-medium mb-1">First Name</label>
              <input
                value={form.firstName}
                onChange={(e) => setForm((f) => ({ ...f, firstName: e.target.value }))}
                required
                className="w-full px-3 py-2 border border-border rounded-lg text-sm"
              />
            </div>
            <div className="flex-1">
              <label className="block text-xs font-medium mb-1">Last Name</label>
              <input
                value={form.lastName}
                onChange={(e) => setForm((f) => ({ ...f, lastName: e.target.value }))}
                required
                className="w-full px-3 py-2 border border-border rounded-lg text-sm"
              />
            </div>
            <div className="w-36">
              <label className="block text-xs font-medium mb-1">Position</label>
              <select
                value={form.position}
                onChange={(e) => setForm((f) => ({ ...f, position: +e.target.value }))}
                className="w-full px-3 py-2 border border-border rounded-lg text-sm bg-white"
              >
                {Object.entries(POSITION_LABELS).map(([val, label]) => (
                  <option key={val} value={val}>{label}</option>
                ))}
              </select>
            </div>
          </div>
          <div className="flex gap-3 items-end">
            <div className="flex-1">
              <label className="block text-xs font-medium mb-1">Email</label>
              <input
                type="email"
                value={form.email}
                onChange={(e) => setForm((f) => ({ ...f, email: e.target.value }))}
                required
                className="w-full px-3 py-2 border border-border rounded-lg text-sm"
                placeholder="employee@company.com"
              />
            </div>
            <div className="w-48">
              <label className="block text-xs font-medium mb-1">Business Unit</label>
              <select
                value={form.unitId}
                onChange={(e) => setForm((f) => ({ ...f, unitId: e.target.value }))}
                required
                className="w-full px-3 py-2 border border-border rounded-lg text-sm bg-white"
              >
                <option value="">Select BU...</option>
                {buList.map((bu) => (
                  <option key={bu.id} value={bu.id}>{bu.businessUnitName}</option>
                ))}
              </select>
            </div>
            {isOwner && (
              <div className="w-32">
                <label className="block text-xs font-medium mb-1">Role</label>
                <select
                  value={form.role}
                  onChange={(e) => setForm((f) => ({ ...f, role: +e.target.value }))}
                  className="w-full px-3 py-2 border border-border rounded-lg text-sm bg-white"
                >
                  {Object.entries(ROLE_LABELS).map(([val, label]) => (
                    <option key={val} value={val}>{label}</option>
                  ))}
                </select>
              </div>
            )}
            <button
              type="submit"
              disabled={submitting}
              className="bg-success text-white px-4 py-2 rounded-lg text-sm hover:opacity-90 disabled:opacity-50"
            >
              {submitting ? "Creating..." : "Create"}
            </button>
          </div>
        </form>
      )}

      {created && (
        <div className="bg-green-50 border border-green-200 rounded-xl p-4 mb-6">
          <h3 className="font-semibold text-green-800 mb-2">Employee Created</h3>
          <p className="text-sm text-green-700">Employee ID: <span className="font-mono font-bold">{created.employeeId}</span></p>
          <p className="text-sm text-green-700">Generated Password: <span className="font-mono font-bold">{created.generatedPassword}</span></p>
          <p className="text-xs text-green-600 mt-2">Share these credentials with the employee so they can log in.</p>
        </div>
      )}

      {loading ? (
        <p className="text-muted">Loading...</p>
      ) : list.length === 0 ? (
        <p className="text-muted">No employees yet.</p>
      ) : (
        <div className="bg-card border border-border rounded-xl overflow-hidden">
          <table className="w-full text-sm">
            <thead className="bg-gray-50 border-b border-border">
              <tr>
                <th className="text-left px-4 py-3 font-medium">ID</th>
                <th className="text-left px-4 py-3 font-medium">Name</th>
                <th className="text-left px-4 py-3 font-medium">Email</th>
                <th className="text-left px-4 py-3 font-medium">Position</th>
                <th className="text-left px-4 py-3 font-medium">Business Unit</th>
                <th className="text-left px-4 py-3 font-medium">Role</th>
                <th className="text-right px-4 py-3 font-medium">Actions</th>
              </tr>
            </thead>
            <tbody>
              {list.map((emp) => (
                <tr key={emp.id} className="border-b border-border last:border-0">
                  <td className="px-4 py-3 text-muted">{emp.id}</td>
                  <td className="px-4 py-3">{emp.firstName} {emp.lastName}</td>
                  <td className="px-4 py-3 text-muted">{emp.email || "-"}</td>
                  <td className="px-4 py-3 text-muted">
                    {emp.position ? POSITION_LABELS[emp.position] || `Unknown (${emp.position})` : "-"}
                  </td>
                  <td className="px-4 py-3">
                    {isOwner && emp.employeeDataId ? (
                      <select
                        value={emp.unitId ?? 0}
                        onChange={(e) => handleUnitChange(emp, +e.target.value)}
                        className="px-2 py-1 border border-border rounded text-xs bg-white"
                      >
                        <option value={0}>Unassigned</option>
                        {buList.map((bu) => (
                          <option key={bu.id} value={bu.id}>{bu.businessUnitName}</option>
                        ))}
                      </select>
                    ) : (
                      <span className={`text-xs px-2 py-0.5 rounded ${!emp.unitId ? "bg-red-100 text-red-700" : "bg-gray-100 text-gray-600"}`}>
                        {emp.unitId ? buList.find((bu) => bu.id === emp.unitId)?.businessUnitName || `BU #${emp.unitId}` : "Unassigned"}
                      </span>
                    )}
                  </td>
                  <td className="px-4 py-3">
                    {isOwner && emp.employeeDataId ? (
                      <select
                        value={emp.role ?? 0}
                        onChange={(e) => handleRoleChange(emp, +e.target.value)}
                        className="px-2 py-1 border border-border rounded text-xs bg-white"
                      >
                        {Object.entries(ROLE_LABELS).map(([val, label]) => (
                          <option key={val} value={val}>{label}</option>
                        ))}
                      </select>
                    ) : (
                      <span className={`text-xs px-2 py-0.5 rounded ${emp.role === 1 ? "bg-blue-100 text-blue-700" : "bg-gray-100 text-gray-600"}`}>
                        {ROLE_LABELS[emp.role ?? 0] || "Employee"}
                      </span>
                    )}
                  </td>
                  <td className="px-4 py-3 text-right">
                    {canDeleteEmployee(emp) && (
                      <button onClick={() => handleDelete(emp.id)} className="text-danger text-xs hover:underline">
                        Delete
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
