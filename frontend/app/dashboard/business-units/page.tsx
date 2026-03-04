"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/lib/auth-context";
import { businessUnits } from "@/lib/api";
import type { BusinessUnitItem } from "@/lib/types";

export default function BusinessUnitsPage() {
  const { companyId, ownerId, canManage, isOwner } = useAuth();
  const router = useRouter();
  const [list, setList] = useState<BusinessUnitItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [showForm, setShowForm] = useState(false);
  const [name, setName] = useState("");
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    if (!canManage) router.push("/dashboard/chat");
  }, [canManage, router]);

  const load = async () => {
    if (!companyId) return;
    setLoading(true);
    const res = await businessUnits.list(companyId);
    setList(res.businessUnits || []);
    setLoading(false);
  };

  useEffect(() => {
    load();
  }, [companyId]); //eslint-disable-line react-hooks/exhaustive-deps

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    await businessUnits.insert({
      businessUnitName: name,
      companyId: companyId!,
      companyOwnerId: ownerId!,
    });
    setName("");
    setShowForm(false);
    setSubmitting(false);
    load();
  };

  const handleDelete = async (id: number) => {
    await businessUnits.delete(id);
    load();
  };

  const filtered = list.filter((bu) =>
    bu.businessUnitName.toLowerCase().includes(search.toLowerCase())
  );

  if (!canManage) return null;

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold">Business Units</h1>
        <button
          onClick={() => setShowForm(!showForm)}
          className="bg-primary text-white px-4 py-2 rounded-lg text-sm hover:bg-primary-hover transition-colors"
        >
          {showForm ? "Cancel" : "Create BU"}
        </button>
      </div>

      {showForm && (
        <form onSubmit={handleCreate} className="bg-card border border-border rounded-xl p-4 mb-6 flex gap-3 items-end">
          <div className="flex-1">
            <label className="block text-xs font-medium mb-1">Business Unit Name</label>
            <input
              value={name}
              onChange={(e) => setName(e.target.value)}
              required
              className="w-full px-3 py-2 border border-border rounded-lg text-sm"
              placeholder="e.g. Engineering"
            />
          </div>
          <button
            type="submit"
            disabled={submitting}
            className="bg-success text-white px-4 py-2 rounded-lg text-sm hover:opacity-90 disabled:opacity-50"
          >
            Create
          </button>
        </form>
      )}

      <div className="mb-6">
        <input
          type="text"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          placeholder="Search by name..."
          className="px-3 py-2 border border-border rounded-lg text-sm w-64"
        />
      </div>

      {loading ? (
        <p className="text-muted">Loading...</p>
      ) : filtered.length === 0 ? (
        <p className="text-muted">{search ? "No matching business units." : "No business units yet."}</p>
      ) : (
        <div className="bg-card border border-border rounded-xl overflow-hidden">
          <table className="w-full text-sm">
            <thead className="bg-gray-50 border-b border-border">
              <tr>
                <th className="text-left px-4 py-3 font-medium">ID</th>
                <th className="text-left px-4 py-3 font-medium">Name</th>
                <th className="text-right px-4 py-3 font-medium">Actions</th>
              </tr>
            </thead>
            <tbody>
              {filtered.map((bu) => (
                <tr key={bu.id} className="border-b border-border last:border-0">
                  <td className="px-4 py-3 text-muted">{bu.id}</td>
                  <td className="px-4 py-3">{bu.businessUnitName}</td>
                  <td className="px-4 py-3 text-right">
                    {isOwner && (
                      <button
                        onClick={() => handleDelete(bu.id)}
                        className="text-danger text-xs hover:underline"
                      >
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
