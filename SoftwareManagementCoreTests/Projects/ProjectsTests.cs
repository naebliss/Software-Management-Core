﻿using DateTimeShared;
using Moq;
using ProjectsShared;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Projects
{
    [Trait("Entity","Project")]
    public class ProjectsTests
    {
        [Fact(DisplayName = "Delete")]
        public void CanDeleteProject()
        {
            var repoMock = new Mock<IProjectStateRepository>();
            var sut = new ProjectService(repoMock.Object, new DateTimeProvider());
            var guid = Guid.NewGuid();

            sut.DeleteProject(guid);

            repoMock.Verify(s => s.DeleteProjectState(guid));
        }
    }
}