// app.js - dieu khien man hinh dang nhap, dieu huong, va render du lieu

const loginScreen = document.getElementById("loginScreen");
const appShell = document.getElementById("appShell");
const loginForm = document.getElementById("loginForm");
const loginError = document.getElementById("loginError");

document.addEventListener("DOMContentLoaded", () => {
  if (Api.getToken()) {
    showApp();
  }
});

// ---------------- LOGIN ----------------
loginForm.addEventListener("submit", async (e) => {
  e.preventDefault();
  loginError.textContent = "";
  const studentCode = document.getElementById("loginStudentCode").value.trim();
  const password = document.getElementById("loginPassword").value;

  try {
    const data = await Api.login(studentCode, password);
    Api.setToken(data.token);
    sessionStorage.setItem("sp_fullname", data.fullName);
    showApp();
  } catch (err) {
    loginError.textContent = err.message;
  }
});

document.getElementById("logoutBtn").addEventListener("click", () => {
  Api.clearToken();
  sessionStorage.removeItem("sp_fullname");
  appShell.classList.add("hidden");
  loginScreen.classList.remove("hidden");
});

function showApp() {
  loginScreen.classList.add("hidden");
  appShell.classList.remove("hidden");
  document.getElementById("topUserName").textContent =
    sessionStorage.getItem("sp_fullname") || "Sinh viên";
  loadPage("profile");
}

// ---------------- NAVIGATION ----------------
document.querySelectorAll(".nav-item").forEach((btn) => {
  btn.addEventListener("click", () => {
    document.querySelectorAll(".nav-item").forEach((b) => b.classList.remove("active"));
    btn.classList.add("active");
    const page = btn.dataset.page;
    document.querySelectorAll(".page").forEach((p) => p.classList.add("hidden"));
    document.getElementById(`page-${page}`).classList.remove("hidden");
    loadPage(page);
  });
});

const loadedPages = new Set();

function loadPage(page) {
  if (loadedPages.has(page)) return; // cache don gian, khong goi lai API
  loadedPages.add(page);

  if (page === "profile") return loadProfile();
  if (page === "curriculum") return loadCurriculum();
  if (page === "grades") return loadGrades();
  if (page === "schedule") return loadSchedule();
  if (page === "registration") return loadRegistration();
  if (page === "tuition") return loadTuition();
}

function showError(containerId, err) {
  document.getElementById(containerId).innerHTML =
    `<div class="error-msg">Không tải được dữ liệu: ${err.message}</div>`;
}

// ---- Toast thong bao thanh cong / that bai (dung chung toan app) ----
function showToast(message, type = "success") {
  const container = document.getElementById("toastContainer");
  const toast = document.createElement("div");
  toast.className = `toast ${type}`;
  toast.innerHTML = `<span>${type === "success" ? "✅" : "❌"}</span><span>${message}</span>`;
  container.appendChild(toast);
  setTimeout(() => toast.remove(), 3500);
}

// ---------------- PROFILE PAGE ----------------
let currentProfileData = null;

async function loadProfile() {
  try {
    const p = await Api.getProfile();
    currentProfileData = p;
    const initial = p.fullName?.charAt(0) || "?";
    const family = p.familyContacts
      .map(
        (f, i) => `
        <p><b>${i + 1}) Họ và tên</b>${f.fullName}${f.relationship ? " (" + f.relationship + ")" : ""}</p>
        ${f.phone ? `<p><b>Số điện thoại</b>${f.phone}</p>` : ""}
      `
      )
      .join("");

    document.getElementById("profileBody").innerHTML = `
      <div class="profile-grid">
        <div class="profile-avatar-col">
          <div class="avatar-circle">${initial}</div>
          <div class="profile-name">${p.fullName}</div>
          <div class="profile-code">${p.studentCode} - Chính quy</div>
          <span class="badge-gender">${p.gender === "Nam" ? "♂" : "♀"} ${p.gender}</span>
        </div>
        <div class="profile-col">
          <p><b>Ngày sinh</b>${p.dateOfBirth}</p>
          <p><b>Nơi sinh</b>${p.placeOfBirth || "—"}</p>
          <p><b>SĐT</b>${p.phone || "—"}</p>
          <p><b>CMND/CCCD</b>${p.identityCard || "—"}</p>
          <p><b>Mã thẻ BHYT</b>${p.insuranceCode || "—"}</p>
          <p><b>Dân tộc</b>${p.ethnicity || "—"}</p>
          <p><b>Khóa</b>${p.course}</p>
        </div>
        <div class="profile-col">
          <p><b>Email cá nhân</b><a href="mailto:${p.personalEmail}">${p.personalEmail || "—"}</a></p>
          <p><b>Email sinh viên</b><a href="mailto:${p.schoolEmail}">${p.schoolEmail || "—"}</a></p>
          <p><b>Lớp nghề</b>${p.classCode}</p>
          <p><b>Nghề</b>${p.majorName}</p>
          <p><b>Trạng thái</b>${p.status}</p>
        </div>
        <div class="profile-col">
          <p><b>Hộ khẩu thường trú</b>${p.permanentAddress || "—"}</p>
          <p><b>Địa chỉ bố mẹ đang ở</b>${p.parentAddress || "—"}</p>
          <p><b>TT tài khoản ngân hàng</b>${p.bankName || "—"} ${p.bankAccountNumber ? "- " + p.bankAccountNumber : ""}</p>
          <p><b>TT liên hệ gia đình</b></p>
          ${family || "<p>Không có dữ liệu</p>"}
        </div>
      </div>
    `;
  } catch (err) {
    showError("profileBody", err);
  }
}

// ---- Modal Sua thong tin ca nhan ----
const modalEditProfile = document.getElementById("modalEditProfile");
const formEditProfile = document.getElementById("formEditProfile");
const editProfileError = document.getElementById("editProfileError");
const familyContactsList = document.getElementById("familyContactsList");

function renderFamilyContactRow(contact = { fullName: "", relationship: "", phone: "" }) {
  const row = document.createElement("div");
  row.className = "family-contact-row";
  row.innerHTML = `
    <input type="text" class="fc-name" placeholder="Họ và tên" value="${contact.fullName || ""}">
    <input type="text" class="fc-relationship" placeholder="Quan hệ (Bố/Mẹ...)" value="${contact.relationship || ""}">
    <input type="text" class="fc-phone" placeholder="Số điện thoại" value="${contact.phone || ""}">
    <button type="button" class="btn-remove-contact" title="Xoá người này">✕</button>
  `;
  row.querySelector(".btn-remove-contact").addEventListener("click", () => row.remove());
  familyContactsList.appendChild(row);
}

document.getElementById("btnOpenEditProfile").addEventListener("click", () => {
  if (!currentProfileData) return;
  editProfileError.textContent = "";

  document.getElementById("editDateOfBirth").value = toInputDate(currentProfileData.dateOfBirth);
  document.getElementById("editPlaceOfBirth").value = currentProfileData.placeOfBirth || "";
  document.getElementById("editIdentityCard").value = currentProfileData.identityCard || "";
  document.getElementById("editInsuranceCode").value = currentProfileData.insuranceCode || "";
  document.getElementById("editEthnicity").value = currentProfileData.ethnicity || "";
  document.getElementById("editStatus").value = currentProfileData.status || "Đang học";

  document.getElementById("editPhone").value = currentProfileData.phone || "";
  document.getElementById("editPersonalEmail").value = currentProfileData.personalEmail || "";
  document.getElementById("editSchoolEmail").value = currentProfileData.schoolEmail || "";
  document.getElementById("editPermanentAddress").value = currentProfileData.permanentAddress || "";
  document.getElementById("editParentAddress").value = currentProfileData.parentAddress || "";
  document.getElementById("editBankName").value = currentProfileData.bankName || "";
  document.getElementById("editBankAccountNumber").value = currentProfileData.bankAccountNumber || "";
  document.getElementById("editBankAccountHolder").value = currentProfileData.bankAccountHolder || "";

  familyContactsList.innerHTML = "";
  if (currentProfileData.familyContacts.length > 0) {
    currentProfileData.familyContacts.forEach((f) => renderFamilyContactRow(f));
  } else {
    renderFamilyContactRow();
  }

  modalEditProfile.classList.remove("hidden");
});

document.getElementById("btnAddFamilyContact").addEventListener("click", () => {
  renderFamilyContactRow();
});

document.getElementById("btnCancelEditProfile").addEventListener("click", () => {
  modalEditProfile.classList.add("hidden");
});

formEditProfile.addEventListener("submit", async (e) => {
  e.preventDefault();
  editProfileError.textContent = "";

  const familyContacts = [...familyContactsList.querySelectorAll(".family-contact-row")]
    .map((row) => ({
      fullName: row.querySelector(".fc-name").value.trim(),
      relationship: row.querySelector(".fc-relationship").value.trim(),
      phone: row.querySelector(".fc-phone").value.trim()
    }))
    .filter((f) => f.fullName !== "");

  const payload = {
    dateOfBirth: document.getElementById("editDateOfBirth").value,
    placeOfBirth: document.getElementById("editPlaceOfBirth").value.trim(),
    identityCard: document.getElementById("editIdentityCard").value.trim(),
    insuranceCode: document.getElementById("editInsuranceCode").value.trim(),
    ethnicity: document.getElementById("editEthnicity").value.trim(),
    status: document.getElementById("editStatus").value,

    phone: document.getElementById("editPhone").value.trim(),
    personalEmail: document.getElementById("editPersonalEmail").value.trim(),
    schoolEmail: document.getElementById("editSchoolEmail").value.trim(),
    permanentAddress: document.getElementById("editPermanentAddress").value.trim(),
    parentAddress: document.getElementById("editParentAddress").value.trim(),
    bankName: document.getElementById("editBankName").value.trim(),
    bankAccountNumber: document.getElementById("editBankAccountNumber").value.trim(),
    bankAccountHolder: document.getElementById("editBankAccountHolder").value.trim(),
    familyContacts
  };

  try {
    const updated = await Api.updateProfile(payload);
    currentProfileData = updated;
    modalEditProfile.classList.add("hidden");
    loadedPages.delete("profile");
    loadProfile();
  } catch (err) {
    editProfileError.textContent = err.message;
  }
});

// ---------------- CURRICULUM PAGE ----------------
async function loadCurriculum() {
  try {
    const list = await Api.getCurriculum();
    const rows = list
      .map(
        (s) => `
        <tr>
          <td>${s.stt}</td>
          <td>${s.subjectCode}</td>
          <td>${s.subjectName}</td>
          <td>${s.shortName || ""}</td>
          <td>${s.subjectType}</td>
          <td>${s.creditsLT}</td>
          <td>${s.creditsTH}</td>
          <td>${s.periodsLT}</td>
          <td>${s.periodsTH}</td>
          <td>${s.semester}</td>
          <td class="row-actions">
            <button class="btn-icon" onclick="openEditSubject(${s.curriculumItemId}, ${s.semester})">Sửa</button>
            <button class="btn-icon danger" onclick="deleteCurriculumRow(${s.curriculumItemId})">Xoá</button>
          </td>
        </tr>`
      )
      .join("");

    document.getElementById("curriculumBody").innerHTML = `
      <p><b>Danh sách môn học (${list.length} môn)</b></p>
      <div class="table-scroll">
        <table class="data-table">
          <thead>
            <tr>
              <th>STT</th><th>Mã môn</th><th>Tên môn</th><th>Tên viết tắt</th>
              <th>Loại môn</th><th>Số TC LT</th><th>Số TC TH</th>
              <th>Số tiết LT</th><th>Số tiết TH</th><th>Kỳ học</th><th>Thao tác</th>
            </tr>
          </thead>
          <tbody>${rows || `<tr class="empty-row"><td colspan="11">Không có dữ liệu</td></tr>`}</tbody>
        </table>
      </div>
    `;
  } catch (err) {
    showError("curriculumBody", err);
  }
}

// ---- Modal Sua ky hoc ----
const modalEditSubject = document.getElementById("modalEditSubject");
const formEditSubject = document.getElementById("formEditSubject");
const editSubjectError = document.getElementById("editSubjectError");

function openEditSubject(curriculumItemId, semester) {
  editSubjectError.textContent = "";
  document.getElementById("editCurriculumItemId").value = curriculumItemId;
  document.getElementById("editSubjectSemester").value = semester;
  modalEditSubject.classList.remove("hidden");
}

document.getElementById("btnCancelEditSubject").addEventListener("click", () => {
  modalEditSubject.classList.add("hidden");
});

formEditSubject.addEventListener("submit", async (e) => {
  e.preventDefault();
  editSubjectError.textContent = "";
  const id = document.getElementById("editCurriculumItemId").value;
  const semester = parseInt(document.getElementById("editSubjectSemester").value, 10);

  try {
    await Api.updateCurriculumSubject(id, { semester });
    modalEditSubject.classList.add("hidden");
    loadedPages.delete("curriculum");
    loadCurriculum();
  } catch (err) {
    editSubjectError.textContent = err.message;
  }
});

async function deleteCurriculumRow(id) {
  if (!confirm("Xoá môn học này khỏi chương trình đào tạo?")) return;
  try {
    await Api.deleteCurriculumSubject(id);
    loadedPages.delete("curriculum");
    loadCurriculum();
  } catch (err) {
    alert("Xoá thất bại: " + err.message);
  }
}

// ---------------- GRADES PAGE ----------------
async function loadGrades() {
  try {
    const t = await Api.getGrades();

    const conductRows = t.conductScores
      .map((c) => `<td>${c.score}</td>`)
      .join("");
    const conductHeaders = t.conductScores
      .map((c) => `<th>Điểm rèn luyện năm ${c.academicYear}</th>`)
      .join("");

    const failedRows = t.failedSubjects
      .map(
        (g) => `
        <tr><td>${g.subjectCode}</td><td>${g.subjectName}</td>
        <td>${g.creditsLT}</td><td>${g.creditsTH}</td></tr>`
      )
      .join("");

    const warningHtml =
      t.warningLevel && t.warningLevel !== "none"
        ? `<div class="warning-banner ${t.warningLevel}">
             <span class="icon">${t.warningLevel === "severe" ? "🚨" : "⚠️"}</span>
             <span>${t.warningMessage}</span>
           </div>`
        : "";

    const semesterBlocks = t.semesters
      .map((sem) => {
        const rows = sem.subjects
          .map(
            (g) => `
          <tr>
            <td>${g.subjectCode}</td>
            <td>${g.subjectName}</td>
            <td>${g.creditsLT}</td>
            <td>${g.creditsTH}</td>
            <td>${g.midtermLT ?? "-"}</td>
            <td>${g.midtermTH ?? "-"}</td>
            <td>${g.finalLT ?? "-"}</td>
            <td>${g.finalTH ?? "-"}</td>
            <td>${g.averageScore ?? "-"}</td>
            <td class="row-actions">
              <button class="btn-icon" onclick='openEditGrade(${JSON.stringify(g)})'>Sửa</button>
              <button class="btn-icon danger" onclick="deleteGradeRow(${g.gradeId})">Xoá</button>
            </td>
          </tr>`
          )
          .join("");

        return `
        <div class="section-title">★ Học kỳ ${sem.semester} ★</div>
        <div class="table-scroll">
          <table class="data-table">
            <thead>
              <tr>
                <th>Mã môn</th><th>Tên môn</th><th>Số TC LT</th><th>Số TC TH</th>
                <th>Điểm giữa kỳ LT</th><th>Điểm giữa kỳ TH</th>
                <th>Điểm LT cao nhất</th><th>Điểm TH cao nhất</th><th>Điểm TB</th><th>Thao tác</th>
              </tr>
            </thead>
            <tbody>${rows}</tbody>
            <tfoot>
              <tr><td colspan="8" class="semester-avg">Điểm TB kỳ</td>
              <td class="semester-avg"><span class="value">${sem.semesterAverage}</span></td><td></td></tr>
            </tfoot>
          </table>
        </div>`;
      })
      .join("");

    document.getElementById("gradesBody").innerHTML = `
      ${warningHtml}

      <div class="section-title">🏆 Điểm rèn luyện 🏆</div>
      <div class="table-scroll">
        <table class="data-table">
          <thead><tr>${conductHeaders}<th>Điểm rèn luyện TB</th><th>Đánh giá</th></tr></thead>
          <tbody><tr>${conductRows}<td>${t.conductAverage}</td>
          <td>${t.conductScores[0]?.rating || "-"}</td></tr></tbody>
        </table>
      </div>

      <div class="section-title">🍑 Danh sách môn không qua 🍑</div>
      <div class="table-scroll">
        <table class="data-table">
          <thead><tr><th>Mã môn</th><th>Tên môn</th><th>Số TC LT</th><th>Số TC TH</th></tr></thead>
          <tbody>${failedRows || `<tr class="empty-row"><td colspan="4">Không có môn học nào</td></tr>`}</tbody>
        </table>
      </div>

      ${semesterBlocks}
    `;
  } catch (err) {
    showError("gradesBody", err);
  }
}

// ---- Modal Sua diem ----
const modalEditGrade = document.getElementById("modalEditGrade");
const formEditGrade = document.getElementById("formEditGrade");
const editGradeError = document.getElementById("editGradeError");

function openEditGrade(g) {
  editGradeError.textContent = "";
  document.getElementById("editGradeId").value = g.gradeId;
  document.getElementById("editGradeMidtermLT").value = g.midtermLT ?? "";
  document.getElementById("editGradeMidtermTH").value = g.midtermTH ?? "";
  document.getElementById("editGradeFinalLT").value = g.finalLT ?? "";
  document.getElementById("editGradeFinalTH").value = g.finalTH ?? "";
  modalEditGrade.classList.remove("hidden");
}

document.getElementById("btnCancelEditGrade").addEventListener("click", () => {
  modalEditGrade.classList.add("hidden");
});

formEditGrade.addEventListener("submit", async (e) => {
  e.preventDefault();
  editGradeError.textContent = "";
  const id = document.getElementById("editGradeId").value;

  const toNumberOrNull = (elId) => {
    const v = document.getElementById(elId).value;
    return v === "" ? null : parseFloat(v);
  };

  try {
    await Api.updateGrade(id, {
      midtermLT: toNumberOrNull("editGradeMidtermLT"),
      midtermTH: toNumberOrNull("editGradeMidtermTH"),
      finalLT: toNumberOrNull("editGradeFinalLT"),
      finalTH: toNumberOrNull("editGradeFinalTH")
    });
    modalEditGrade.classList.add("hidden");
    loadedPages.delete("grades");
    loadGrades();
  } catch (err) {
    editGradeError.textContent = err.message;
  }
});

async function deleteGradeRow(id) {
  if (!confirm("Xoá điểm môn học này?")) return;
  try {
    await Api.deleteGrade(id);
    loadedPages.delete("grades");
    loadGrades();
  } catch (err) {
    alert("Xoá thất bại: " + err.message);
  }
}

// ---------------- MODAL: THÊM MÔN HỌC ----------------
const modalAddSubject = document.getElementById("modalAddSubject");
const formAddSubject = document.getElementById("formAddSubject");
const selectExistingSubject = document.getElementById("selectExistingSubject");
const addSubjectError = document.getElementById("addSubjectError");

document.getElementById("btnOpenAddSubject").addEventListener("click", async () => {
  addSubjectError.textContent = "";
  formAddSubject.reset();
  modalAddSubject.classList.remove("hidden");
  try {
    const subjects = await Api.getAllSubjects();
    selectExistingSubject.innerHTML =
      `<option value="">— Chọn môn học —</option>` +
      subjects
        .map((s) => `<option value="${s.subjectId}">${s.subjectCode} - ${s.subjectName}</option>`)
        .join("");
  } catch (err) {
    addSubjectError.textContent = "Không tải được danh sách môn: " + err.message;
  }
});

document.getElementById("btnCancelAddSubject").addEventListener("click", () => {
  modalAddSubject.classList.add("hidden");
});

formAddSubject.addEventListener("submit", async (e) => {
  e.preventDefault();
  addSubjectError.textContent = "";

  const semester = parseInt(document.getElementById("subjectSemester").value, 10);
  const existingId = selectExistingSubject.value;

  try {
    let subjectId;

    if (existingId) {
      subjectId = parseInt(existingId, 10);
    } else {
      // Tạo môn mới trước, rồi lấy id vừa tạo
      const code = document.getElementById("newSubjectCode").value.trim();
      const name = document.getElementById("newSubjectName").value.trim();
      if (!code || !name) {
        throw new Error("Vui lòng chọn môn có sẵn, hoặc nhập đủ Mã môn + Tên môn để tạo môn mới.");
      }
      const created = await Api.createSubject({
        subjectCode: code,
        subjectName: name,
        creditsLT: parseInt(document.getElementById("newSubjectCreditsLT").value || "0", 10),
        creditsTH: parseInt(document.getElementById("newSubjectCreditsTH").value || "0", 10),
        periodsLT: parseInt(document.getElementById("newSubjectPeriodsLT").value || "0", 10),
        periodsTH: parseInt(document.getElementById("newSubjectPeriodsTH").value || "0", 10)
      });
      subjectId = created.subjectId;
    }

    await Api.addCurriculumSubject({ subjectId, semester });

    modalAddSubject.classList.add("hidden");
    loadedPages.delete("curriculum"); // buoc tai lai du lieu moi
    loadCurriculum();
  } catch (err) {
    addSubjectError.textContent = err.message;
  }
});

// ---------------- MODAL: THÊM ĐIỂM ----------------
const modalAddGrade = document.getElementById("modalAddGrade");
const formAddGrade = document.getElementById("formAddGrade");
const selectGradeSubject = document.getElementById("selectGradeSubject");
const addGradeError = document.getElementById("addGradeError");

document.getElementById("btnOpenAddGrade").addEventListener("click", async () => {
  addGradeError.textContent = "";
  formAddGrade.reset();
  document.getElementById("gradeSemester").value = 1;
  modalAddGrade.classList.remove("hidden");
  try {
    const subjects = await Api.getAllSubjects();
    selectGradeSubject.innerHTML =
      `<option value="">— Chọn môn học —</option>` +
      subjects
        .map((s) => `<option value="${s.subjectId}">${s.subjectCode} - ${s.subjectName}</option>`)
        .join("");
  } catch (err) {
    addGradeError.textContent = "Không tải được danh sách môn: " + err.message;
  }
});

document.getElementById("btnCancelAddGrade").addEventListener("click", () => {
  modalAddGrade.classList.add("hidden");
});

formAddGrade.addEventListener("submit", async (e) => {
  e.preventDefault();
  addGradeError.textContent = "";

  const subjectId = parseInt(selectGradeSubject.value, 10);
  if (!subjectId) {
    addGradeError.textContent = "Vui lòng chọn môn học.";
    return;
  }

  const toNumberOrNull = (id) => {
    const v = document.getElementById(id).value;
    return v === "" ? null : parseFloat(v);
  };

  try {
    await Api.addGrade({
      subjectId,
      semester: parseInt(document.getElementById("gradeSemester").value, 10),
      midtermLT: toNumberOrNull("gradeMidtermLT"),
      midtermTH: toNumberOrNull("gradeMidtermTH"),
      finalLT: toNumberOrNull("gradeFinalLT"),
      finalTH: toNumberOrNull("gradeFinalTH")
    });

    modalAddGrade.classList.add("hidden");
    loadedPages.delete("grades"); // buoc tai lai du lieu moi
    loadGrades();
  } catch (err) {
    addGradeError.textContent = err.message;
  }
});

// ---------------- XUẤT PDF (chỉ frontend, dùng jsPDF + html2canvas) ----------------
document.getElementById("btnExportPdf").addEventListener("click", async () => {
  const btn = document.getElementById("btnExportPdf");
  const original = btn.textContent;
  btn.textContent = "Đang xuất...";
  btn.disabled = true;

  try {
    const target = document.getElementById("gradesBody");
    const canvas = await html2canvas(target, { scale: 2, backgroundColor: "#ffffff" });
    const imgData = canvas.toDataURL("image/png");

    const { jsPDF } = window.jspdf;
    const pdf = new jsPDF("p", "mm", "a4");
    const pageWidth = pdf.internal.pageSize.getWidth();
    const pageHeight = pdf.internal.pageSize.getHeight();

    const imgWidth = pageWidth - 20; // le 10mm moi ben
    const imgHeight = (canvas.height * imgWidth) / canvas.width;

    let heightLeft = imgHeight;
    let position = 10;

    pdf.addImage(imgData, "PNG", 10, position, imgWidth, imgHeight);
    heightLeft -= pageHeight - 20;

    while (heightLeft > 0) {
      position = heightLeft - imgHeight + 10;
      pdf.addPage();
      pdf.addImage(imgData, "PNG", 10, position, imgWidth, imgHeight);
      heightLeft -= pageHeight - 20;
    }

    const studentName = sessionStorage.getItem("sp_fullname") || "SinhVien";
    pdf.save(`BangDiem_${studentName.replace(/\s+/g, "_")}.pdf`);
  } catch (err) {
    alert("Xuất PDF thất bại: " + err.message);
  } finally {
    btn.textContent = original;
    btn.disabled = false;
  }
});

// ---------------- SCHEDULE PAGE ----------------
async function loadSchedule() {
  try {
    const list = await Api.getSchedule();
    const rows = list
      .map(
        (s) => `
        <tr>
          <td>${s.dayOfWeek}</td>
          <td>${s.classDate}</td>
          <td>Tuần ${s.weekNumber}</td>
          <td>${s.semester}</td>
          <td>${s.academicYear}</td>
          <td>${s.subjectName}</td>
          <td>${s.startPeriod}</td>
          <td>${s.endPeriod}</td>
          <td>${s.room}</td>
          <td>${s.lecturer}</td>
          <td class="row-actions">
            <button class="btn-icon" onclick='openEditSchedule(${JSON.stringify(s)})'>Sửa</button>
            <button class="btn-icon danger" onclick="deleteScheduleRow(${s.scheduleId})">Xoá</button>
          </td>
        </tr>`
      )
      .join("");

    document.getElementById("scheduleBody").innerHTML = `
      <div class="table-scroll">
        <table class="data-table">
          <thead>
            <tr>
              <th>Thứ</th><th>Ngày</th><th>Tuần</th><th>Học kỳ</th><th>Năm học</th>
              <th>Môn học</th><th>Bắt đầu</th><th>Kết thúc</th><th>Phòng</th><th>Giảng viên</th><th>Thao tác</th>
            </tr>
          </thead>
          <tbody>${rows || `<tr class="empty-row"><td colspan="11">Không có lịch học</td></tr>`}</tbody>
        </table>
      </div>
    `;
  } catch (err) {
    showError("scheduleBody", err);
  }
}

// ---- Modal Them / Sua buoi hoc ----
const modalSchedule = document.getElementById("modalSchedule");
const formSchedule = document.getElementById("formSchedule");
const scheduleSubjectSelect = document.getElementById("scheduleSubject");
const scheduleError = document.getElementById("scheduleError");
let isRegistrationMode = false; // true khi mo modal tu trang "Dang ky lich hoc"

async function populateScheduleSubjects(selectedId) {
  try {
    const subjects = await Api.getAllSubjects();
    scheduleSubjectSelect.innerHTML =
      `<option value="">— Chọn môn học —</option>` +
      subjects
        .map(
          (s) =>
            `<option value="${s.subjectId}" ${s.subjectId === selectedId ? "selected" : ""}>${s.subjectCode} - ${s.subjectName}</option>`
        )
        .join("");
  } catch (err) {
    scheduleError.textContent = "Không tải được danh sách môn: " + err.message;
  }
}

// "dd/MM/yyyy" -> "yyyy-MM-dd" (dinh dang input[type=date] can)
function toInputDate(dateStr) {
  const [d, m, y] = dateStr.split("/");
  return `${y}-${m}-${d}`;
}

document.getElementById("btnOpenAddSchedule").addEventListener("click", async () => {
  isRegistrationMode = false;
  scheduleError.textContent = "";
  formSchedule.reset();
  document.getElementById("scheduleId").value = "";
  document.getElementById("scheduleModalTitle").textContent = "Thêm buổi học";
  document.getElementById("btnSubmitSchedule").textContent = "Thêm buổi học";
  await populateScheduleSubjects(null);
  modalSchedule.classList.remove("hidden");
});

async function openEditSchedule(s) {
  isRegistrationMode = false;
  scheduleError.textContent = "";
  formSchedule.reset();
  document.getElementById("scheduleModalTitle").textContent = "Sửa buổi học";
  document.getElementById("btnSubmitSchedule").textContent = "Lưu thay đổi";
  await populateScheduleSubjects(s.subjectId);

  document.getElementById("scheduleId").value = s.scheduleId;
  document.getElementById("scheduleDate").value = toInputDate(s.classDate);
  document.getElementById("scheduleWeek").value = s.weekNumber;
  document.getElementById("scheduleSemesterInput").value = s.semester;
  document.getElementById("scheduleAcademicYear").value = s.academicYear;
  document.getElementById("scheduleStartPeriod").value = s.startPeriod;
  document.getElementById("scheduleEndPeriod").value = s.endPeriod;
  document.getElementById("scheduleRoom").value = s.room;
  document.getElementById("scheduleLecturer").value = s.lecturer;

  modalSchedule.classList.remove("hidden");
}

// Mo modal o che do "Dang ky lich hoc": mon hoc duoc chon san, khong cho doi sang mon khac
async function openRegisterSchedule(subjectId, subjectName, semester) {
  isRegistrationMode = true;
  scheduleError.textContent = "";
  formSchedule.reset();
  document.getElementById("scheduleId").value = "";
  document.getElementById("scheduleModalTitle").textContent = `Đăng ký lịch học: ${subjectName}`;
  document.getElementById("btnSubmitSchedule").textContent = "Xác nhận đăng ký";
  await populateScheduleSubjects(subjectId);
  scheduleSubjectSelect.disabled = true;
  document.getElementById("scheduleSemesterInput").value = semester || 1;
  modalSchedule.classList.remove("hidden");
}

document.getElementById("btnCancelSchedule").addEventListener("click", () => {
  scheduleSubjectSelect.disabled = false;
  modalSchedule.classList.add("hidden");
});

formSchedule.addEventListener("submit", async (e) => {
  e.preventDefault();
  scheduleError.textContent = "";

  const id = document.getElementById("scheduleId").value;
  const subjectId = parseInt(scheduleSubjectSelect.value, 10);
  if (!subjectId) {
    scheduleError.textContent = "Vui lòng chọn môn học.";
    return;
  }

  const payload = {
    subjectId,
    classDate: document.getElementById("scheduleDate").value,
    weekNumber: parseInt(document.getElementById("scheduleWeek").value, 10),
    semester: parseInt(document.getElementById("scheduleSemesterInput").value, 10),
    academicYear: document.getElementById("scheduleAcademicYear").value.trim(),
    startPeriod: document.getElementById("scheduleStartPeriod").value.trim(),
    endPeriod: document.getElementById("scheduleEndPeriod").value.trim(),
    room: document.getElementById("scheduleRoom").value.trim(),
    lecturer: document.getElementById("scheduleLecturer").value.trim()
  };

  try {
    if (id) {
      await Api.updateSchedule(id, payload);
      showToast("Đã lưu thay đổi lịch học thành công.", "success");
    } else {
      await Api.addSchedule(payload);
      showToast(
        isRegistrationMode ? "Đăng ký lịch học thành công!" : "Đã thêm buổi học thành công.",
        "success"
      );
    }

    scheduleSubjectSelect.disabled = false;
    modalSchedule.classList.add("hidden");
    loadedPages.delete("schedule");
    loadedPages.delete("registration");

    // Chi tai lai trang dang dang hien thi
    if (!document.getElementById("page-registration").classList.contains("hidden")) {
      loadRegistration();
    } else {
      loadSchedule();
    }
  } catch (err) {
    scheduleError.textContent = err.message;
    showToast(
      isRegistrationMode ? `Đăng ký thất bại: ${err.message}` : `Lưu thất bại: ${err.message}`,
      "error"
    );
  }
});

async function deleteScheduleRow(id) {
  if (!confirm("Xoá buổi học này khỏi lịch học?")) return;
  try {
    await Api.deleteSchedule(id);
    loadedPages.delete("schedule");
    showToast("Đã xoá buổi học.", "success");
    loadSchedule();
  } catch (err) {
    showToast("Xoá thất bại: " + err.message, "error");
  }
}

// ---------------- REGISTRATION PAGE (Dang ky lich hoc) ----------------
async function loadRegistration() {
  try {
    const [curriculum, schedule] = await Promise.all([Api.getCurriculum(), Api.getSchedule()]);

    const scheduledSubjectIds = new Set(schedule.map((s) => s.subjectId));
    const notScheduled = curriculum.filter((c) => {
      const subjectId = c.subjectId ?? null;
      return subjectId ? !scheduledSubjectIds.has(subjectId) : true;
    });

    if (notScheduled.length === 0) {
      document.getElementById("registrationBody").innerHTML = `
        <div class="warning-banner" style="background:#e4f7ee;border:1px solid #b9e6cf;color:var(--green)">
          <span class="icon">✅</span><span>Bạn đã đăng ký lịch học cho tất cả các môn trong chương trình đào tạo.</span>
        </div>`;
      return;
    }

    const cards = notScheduled
      .map(
        (c) => `
        <div class="reg-card">
          <div class="reg-info">
            <b>${c.subjectCode} - ${c.subjectName}</b>
            <span>Kỳ học ${c.semester} · Số TC LT: ${c.creditsLT} · Số TC TH: ${c.creditsTH}</span>
          </div>
          <button class="btn-register" onclick="openRegisterSchedule(${c.subjectId ?? "null"}, '${(c.subjectName || "").replace(/'/g, "\\'")}', ${c.semester})">
            Đăng ký lịch học
          </button>
        </div>`
      )
      .join("");

    document.getElementById("registrationBody").innerHTML = `
      <p style="margin-bottom:14px;color:var(--text-muted);font-size:13.5px">
        Danh sách môn học trong chương trình đào tạo <b>chưa có lịch học</b>. Bấm "Đăng ký lịch học" để chọn ngày/giờ/phòng cho từng môn.
      </p>
      ${cards}
    `;
  } catch (err) {
    showError("registrationBody", err);
  }
}

// ---------------- TUITION PAGE (Học phí) ----------------
async function loadTuition() {
  try {
    const list = await Api.getTuitions();

    const unpaidCount = list.filter((t) => t.status !== "Đã nộp đủ").length;
    const warningHtml =
      unpaidCount > 0
        ? `<div class="warning-banner severe">
             <span class="icon">🚨</span>
             <span>Bạn đang có <b>${unpaidCount}</b> kỳ học phí chưa nộp đủ. Vui lòng nộp học phí đúng hạn để tránh bị khoá tài khoản / không được đăng ký môn học kỳ tiếp theo.</span>
           </div>`
        : `<div class="warning-banner" style="background:#e4f7ee;border:1px solid #b9e6cf;color:var(--green)">
             <span class="icon">✅</span><span>Bạn đã nộp đủ học phí tất cả các kỳ. Không có khoản nào quá hạn.</span>
           </div>`;

    const rows = list
      .map((t) => {
        let badgeClass = "badge-unpaid";
        if (t.status === "Đã nộp đủ") badgeClass = "badge-paid";
        else if (t.status === "Nộp thiếu") badgeClass = "badge-partial";
        else if (t.status === "Quá hạn - Chưa nộp") badgeClass = "badge-overdue";

        return `
        <tr>
          <td>${t.semester}</td>
          <td>${t.academicYear}</td>
          <td>${t.amountDue.toLocaleString("vi-VN")} đ</td>
          <td>${t.amountPaid.toLocaleString("vi-VN")} đ</td>
          <td>${t.amountRemaining.toLocaleString("vi-VN")} đ</td>
          <td>${t.dueDate}</td>
          <td>${t.paidDate || "—"}</td>
          <td><span class="badge-status ${badgeClass}">${t.status}</span></td>
          <td class="row-actions">
            <button class="btn-icon" onclick='openEditTuition(${JSON.stringify(t)})'>Sửa</button>
            <button class="btn-icon danger" onclick="deleteTuitionRow(${t.tuitionId})">Xoá</button>
          </td>
        </tr>`;
      })
      .join("");

    document.getElementById("tuitionBody").innerHTML = `
      ${warningHtml}
      <div class="table-scroll">
        <table class="data-table">
          <thead>
            <tr>
              <th>Học kỳ</th><th>Năm học</th><th>Phải nộp</th><th>Đã nộp</th>
              <th>Còn thiếu</th><th>Hạn nộp</th><th>Ngày nộp</th><th>Trạng thái</th><th>Thao tác</th>
            </tr>
          </thead>
          <tbody>${rows || `<tr class="empty-row"><td colspan="9">Không có dữ liệu học phí</td></tr>`}</tbody>
        </table>
      </div>
    `;
  } catch (err) {
    showError("tuitionBody", err);
  }
}

// ---- Modal Them / Sua hoc phi ----
const modalTuition = document.getElementById("modalTuition");
const formTuition = document.getElementById("formTuition");
const tuitionError = document.getElementById("tuitionError");

document.getElementById("btnOpenAddTuition").addEventListener("click", () => {
  tuitionError.textContent = "";
  formTuition.reset();
  document.getElementById("tuitionId").value = "";
  document.getElementById("tuitionModalTitle").textContent = "Thêm kỳ học phí";
  document.getElementById("btnSubmitTuition").textContent = "Thêm kỳ học phí";
  document.getElementById("tuitionAmountPaid").value = 0;
  modalTuition.classList.remove("hidden");
});

function openEditTuition(t) {
  tuitionError.textContent = "";
  document.getElementById("tuitionModalTitle").textContent = "Sửa kỳ học phí";
  document.getElementById("btnSubmitTuition").textContent = "Lưu thay đổi";

  document.getElementById("tuitionId").value = t.tuitionId;
  document.getElementById("tuitionSemester").value = t.semester;
  document.getElementById("tuitionAcademicYear").value = t.academicYear;
  document.getElementById("tuitionAmountDue").value = t.amountDue;
  document.getElementById("tuitionAmountPaid").value = t.amountPaid;
  document.getElementById("tuitionDueDate").value = toInputDate(t.dueDate);

  modalTuition.classList.remove("hidden");
}

document.getElementById("btnCancelTuition").addEventListener("click", () => {
  modalTuition.classList.add("hidden");
});

formTuition.addEventListener("submit", async (e) => {
  e.preventDefault();
  tuitionError.textContent = "";

  const id = document.getElementById("tuitionId").value;
  const payload = {
    semester: parseInt(document.getElementById("tuitionSemester").value, 10),
    academicYear: document.getElementById("tuitionAcademicYear").value.trim(),
    amountDue: parseFloat(document.getElementById("tuitionAmountDue").value),
    amountPaid: parseFloat(document.getElementById("tuitionAmountPaid").value),
    dueDate: document.getElementById("tuitionDueDate").value
  };

  try {
    if (id) {
      await Api.updateTuition(id, payload);
    } else {
      await Api.addTuition(payload);
    }
    modalTuition.classList.add("hidden");
    loadedPages.delete("tuition");
    loadTuition();
  } catch (err) {
    tuitionError.textContent = err.message;
  }
});

async function deleteTuitionRow(id) {
  if (!confirm("Xoá kỳ học phí này?")) return;
  try {
    await Api.deleteTuition(id);
    loadedPages.delete("tuition");
    loadTuition();
  } catch (err) {
    alert("Xoá thất bại: " + err.message);
  }
}
