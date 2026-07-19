// api.js - lop giao tiep voi Backend C# (ASP.NET Core Web API)

const API_BASE_URL = "http://localhost:5000/api"; // doi lai theo port backend cua ban chay

const Api = {
  getToken() {
    return sessionStorage.getItem("sp_token");
  },
  setToken(token) {
    sessionStorage.setItem("sp_token", token);
  },
  clearToken() {
    sessionStorage.removeItem("sp_token");
  },

  async login(studentCode, password) {
    const res = await fetch(`${API_BASE_URL}/auth/login`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ studentCode, password })
    });
    if (!res.ok) {
      const err = await res.json().catch(() => ({ message: "Dang nhap that bai" }));
      throw new Error(err.message || "Dang nhap that bai");
    }
    return res.json();
  },

  async _authGet(path) {
    const res = await fetch(`${API_BASE_URL}${path}`, {
      headers: { Authorization: `Bearer ${this.getToken()}` }
    });
    if (res.status === 401) {
      Api.clearToken();
      window.location.reload();
      throw new Error("Phien dang nhap het han");
    }
    if (!res.ok) throw new Error(`Loi tai du lieu (${res.status})`);
    return res.json();
  },

  async _authPost(path, body) {
    const res = await fetch(`${API_BASE_URL}${path}`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${this.getToken()}`
      },
      body: JSON.stringify(body)
    });
    if (res.status === 401) {
      Api.clearToken();
      window.location.reload();
      throw new Error("Phien dang nhap het han");
    }
    if (!res.ok) {
      const err = await res.json().catch(() => ({ message: `Loi (${res.status})` }));
      throw new Error(err.message || `Loi (${res.status})`);
    }
    return res.json();
  },

  async _authPut(path, body) {
    const res = await fetch(`${API_BASE_URL}${path}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${this.getToken()}`
      },
      body: JSON.stringify(body)
    });
    if (res.status === 401) {
      Api.clearToken();
      window.location.reload();
      throw new Error("Phien dang nhap het han");
    }
    if (!res.ok) {
      const err = await res.json().catch(() => ({ message: `Loi (${res.status})` }));
      throw new Error(err.message || `Loi (${res.status})`);
    }
    if (res.status === 204) return null;
    return res.json();
  },

  async _authDelete(path) {
    const res = await fetch(`${API_BASE_URL}${path}`, {
      method: "DELETE",
      headers: { Authorization: `Bearer ${this.getToken()}` }
    });
    if (res.status === 401) {
      Api.clearToken();
      window.location.reload();
      throw new Error("Phien dang nhap het han");
    }
    if (!res.ok) {
      const err = await res.json().catch(() => ({ message: `Loi (${res.status})` }));
      throw new Error(err.message || `Loi (${res.status})`);
    }
    return true;
  },

  getProfile() { return this._authGet("/student/me"); },
  updateProfile(data) { return this._authPut("/student/me", data); },
  getCurriculum() { return this._authGet("/curriculum"); },
  getGrades() { return this._authGet("/grades"); },
  getSchedule() { return this._authGet("/schedule"); },

  getAllSubjects() { return this._authGet("/subjects"); },
  createSubject(data) { return this._authPost("/subjects", data); },

  addCurriculumSubject(data) { return this._authPost("/curriculum", data); },
  updateCurriculumSubject(id, data) { return this._authPut(`/curriculum/${id}`, data); },
  deleteCurriculumSubject(id) { return this._authDelete(`/curriculum/${id}`); },

  addGrade(data) { return this._authPost("/grades", data); },
  updateGrade(id, data) { return this._authPut(`/grades/${id}`, data); },
  deleteGrade(id) { return this._authDelete(`/grades/${id}`); },

  addSchedule(data) { return this._authPost("/schedule", data); },
  updateSchedule(id, data) { return this._authPut(`/schedule/${id}`, data); },
  deleteSchedule(id) { return this._authDelete(`/schedule/${id}`); },

  getTuitions() { return this._authGet("/tuition"); },
  addTuition(data) { return this._authPost("/tuition", data); },
  updateTuition(id, data) { return this._authPut(`/tuition/${id}`, data); },
  deleteTuition(id) { return this._authDelete(`/tuition/${id}`); }
};
